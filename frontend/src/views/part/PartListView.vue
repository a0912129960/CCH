<script setup lang="ts">
import { authService, UserRole } from '../../services/auth/auth';
import { partService, type PartListItem } from '../../services/part/part';
import { commonService, type CustomerOption, type StatusOption, type SupplierOption } from '../../services/common/common';

// Internal components (Restore manual import for Vitest compatibility)
import Card from '../../components/common/Card.vue';
import Dot from '../../components/common/Dot.vue';
import Button from '../../components/common/Button.vue';

/**
 * Part No List View (零件編號清單頁面)
 * Sidebar layout version.
 * 
 * Audit Update on 2026-04-17 by Gemini AI:
 * Ticket: INTERNAL-AI-20260417
 * Intent: Implement server-side pagination and searching.
 * Impact: UI data binding, sorting, and status/SLA display.
 */

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

// Capture auth state immediately for filtering
const { role, customerId: authCustomerId } = authService.state;
const isEmployee = computed(() => role && role !== UserRole.CUSTOMER);
const userCustomerId = computed(() => authCustomerId);

const parts = ref<PartListItem[]>([]);
const suppliers = ref<SupplierOption[]>([]);
const customers = ref<CustomerOption[]>([]);
const statusOptions = ref<StatusOption[]>([]);
const loading = ref(true);

// Pagination and Search states (分頁與搜尋狀態)
const currentPage = ref(1);
const pageSize = ref(10);
const totalCount = ref(0);

// Search and Filter states
const searchQuery = ref('');
const statusFilter = ref<string>((route.query.status as string) || '');
const supplierFilter = ref('');
const customerFilter = ref<string>((route.query.customerId as string) || (isEmployee.value ? '' : userCustomerId.value || ''));

// Track expanded rows (using number ID now)
const expandedRows = ref<Set<number>>(new Set());

const toggleRow = (id: number) => {
  if (expandedRows.value.has(id)) {
    expandedRows.value.delete(id);
  } else {
    expandedRows.value.add(id);
  }
};

/**
 * Format SLA status to localized text.
 * (將 SLA 狀態格式化為在地化文字。)
 */
const formatSLA = (slaStatus?: string) => {
  if (!slaStatus) return '-';
  return t('sla.' + slaStatus.toLowerCase());
};

/**
 * Reload suppliers based on selected customer (根據所選客戶重新載入供應商)
 */
const reloadSuppliers = async () => {
  const cId = customerFilter.value || 'all';
  suppliers.value = await commonService.getSuppliers(cId);
  if (supplierFilter.value && !suppliers.value.some(s => s.key === supplierFilter.value)) {
    supplierFilter.value = '';
  }
};

/**
 * Fetch parts from backend with current filters and pagination.
 */
const fetchParts = async () => {
  loading.value = true;
  try {
    const result = await partService.getParts({
      customerId: customerFilter.value || undefined,
      status: statusFilter.value || undefined,
      partNo: searchQuery.value || undefined,
      supplier: supplierFilter.value || undefined,
      page: currentPage.value,
      pageSize: pageSize.value
    });
    parts.value = result.data;
    totalCount.value = result.total;
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  try {
    const [customersData, statusesData] = await Promise.all([
      commonService.getCustomers(),
      commonService.getStatusOptions()
    ]);
    customers.value = customersData;
    statusOptions.value = statusesData;

    await reloadSuppliers();

    if (!isEmployee.value && userCustomerId.value) {
      customerFilter.value = userCustomerId.value;
    }

    await fetchParts();
  } catch (error) {
    console.error('Failed to initialize parts list:', error);
    loading.value = false;
  }
});

// Watch for filter/pagination changes (監聽過濾器或分頁變更)
let searchTimeout: any = null;

// 1. Customer Filter change: Reload suppliers AND fetch parts
watch(customerFilter, async () => {
  currentPage.value = 1;
  await reloadSuppliers();
  await fetchParts();
});

// 2. Status or Supplier Filter change: Immediate fetch parts
watch([statusFilter, supplierFilter], async () => {
  currentPage.value = 1;
  await fetchParts();
});

// 3. Search Query change: Debounced fetch parts (1000ms)
watch(searchQuery, () => {
  currentPage.value = 1;
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(async () => {
    await fetchParts();
  }, 1000);
});

watch([currentPage, pageSize], async () => {
  await fetchParts();
});

// const handleSearch = async () => { ... } (Removed)

const getStatusColor = (status: string) => {
  const colors: Record<string, string> = {
    'S01': '#909399', // Unknown
    'S02': '#E6A23C', // Pending Customer
    'S03': '#409EFF', // Pending Review
    'S05': '#F56C6C', // Returned
    'S04': '#67C23A', // Active
    'S06': '#E6A23C', // Flagged
    'S07': '#909399'  // Superseded
  };
  return colors[status.toUpperCase()] || '#909399';
};
</script>

<template>
  <div class="page-wrapper">
    <div class="page-container">
      <header class="page-header">
        <h1>{{ $t('part_list.title') }}</h1>
      </header>

      <Card class="filter-card">
        <div class="filter-grid">
          <!-- Search & Customer Group -->
          <div class="filter-item search">
            <div class="search-input-group">
              <!-- Customer Filter -->
              <div class="group-item">
                <label>{{ $t('employee.customer_select') }}</label>
                <el-select 
                  v-model="customerFilter" 
                  class="form-select-el customer-select" 
                  clearable 
                  filterable
                  :placeholder="$t('employee.customer_select')"
                >
                  <el-option :label="$t('employee.all_customers')" value="" />
                  <el-option v-for="c in customers" :key="c.key" :label="c.value" :value="c.key" />
                </el-select>
              </div>

              <div class="group-item">
                <label>{{ $t('common.search') }}</label>
                <input 
                  v-model="searchQuery" 
                  type="text" 
                  :placeholder="$t('part_list.search_placeholder')"
                  class="form-input"
                />
              </div>
            </div>
            <div class="action-row">
              <Button type="secondary" @click="router.push({ name: 'part-create' })">
                {{ $t('part_list.add_new') }}
              </Button>
              <Button type="secondary" class="ml-4" @click="router.push({ name: 'part-upload' })">
                <i class="el-icon-upload"></i> {{ $t('part_list.bulk_upload') }}
              </Button>
            </div>
          </div>

          <!-- Status Filter -->
          <div class="filter-item">
            <label>{{ $t('part_list.filter_status') }}</label>
            <el-select v-model="statusFilter" class="form-select-el" clearable>
              <el-option :label="$t('common.all')" value="" />
              <el-option 
                v-for="s in statusOptions" 
                :key="s.key" 
                :label="s.value" 
                :value="s.key" 
              />
            </el-select>
          </div>

          <!-- Supplier Filter -->
          <div class="filter-item">
            <label>{{ $t('part_list.filter_supplier') }}</label>
            <el-select v-model="supplierFilter" class="form-select-el" clearable filterable>
              <el-option :label="$t('common.all')" value="" />
              <el-option v-for="s in suppliers" :key="s.key" :label="s.value" :value="s.key" />
            </el-select>
          </div>
        </div>
      </Card>

      <div class="table-wrapper">
        <table class="data-table">
          <thead>
            <tr>
              <th width="40"></th>
              <th>{{ $t('common.customer') }}</th>
              <th>{{ $t('customer.part_no') }}</th>
              <th>{{ $t('part_list.description') }}</th>
              <th>{{ $t('part_detail.country') }}</th>
              <th>{{ $t('customer.hts_code') }}</th>
              <th>{{ $t('part_list.duty_rate') }}</th>
              <th>{{ $t('common.status') }}</th>
              <th>{{ $t('part_list.updated_by') }}</th>
              <th>{{ $t('common.last_updated') }}</th>
              <th>{{ $t('part_list.sla') }}</th>
              <th>{{ $t('common.actions') }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading"><td colspan="12" class="text-center">Loading...</td></tr>
            <template v-else v-for="part in parts" :key="part.id">
              <tr :class="{ 'expanded-row-master': expandedRows.has(part.id) }">
                <td>
                  <span class="expand-toggle" @click="toggleRow(part.id)">
                    <i :class="expandedRows.has(part.id) ? 'el-icon-minus' : 'el-icon-plus'"></i>
                  </span>
                </td>
                <td>{{ part.customer || '-' }}</td>
                <td class="part-no-cell">
                  <a href="javascript:void(0)" @click="router.push({ name: 'part-detail', params: { id: part.id } })">
                    {{ part.partNo }}
                  </a>
                </td>
                <td class="desc-cell">{{ part.partDesc || '-' }}</td>
                <td>{{ part.country || '-' }}</td>
                <td><code>{{ part.htsCode }}</code></td>
                <td>{{ part.rate !== undefined ? part.rate + '%' : '-' }}</td>
                <td>
                  <div class="status-cell">
                    <Dot :color="getStatusColor(part.status)" size="8px" />
                    <span>{{ $t('status.' + part.status.toLowerCase()) }}</span>
                  </div>
                </td>
                <td>{{ part.updatedBy || '-' }}</td>
                <td class="time-cell">{{ part.updatedDate }}</td>
                <td>
                  <span :class="['sla-badge', part.slaStatus ? 'active' : '']">
                    {{ formatSLA(part.slaStatus) }}
                  </span>
                </td>
                <td>
                  <Button type="text" size="small" @click="router.push({ name: 'part-detail', params: { id: part.id } })">
                    View
                  </Button>
                </td>
              </tr>
              <!-- Expandable Detail Row -->
              <tr v-if="expandedRows.has(part.id)" class="detail-row">
                <td colspan="12">
                  <div class="detail-content">
                    <div class="duty-grid">
                      <div class="duty-item">
                        <div class="duty-label">HTS 1</div>
                        <div class="duty-val">Code: {{ part.htsCode1 || '-' }}</div>
                        <div class="duty-val">Rate: {{ part.rate1 !== null && part.rate1 !== undefined ? part.rate1 + '%' : '-' }}</div>
                      </div>
                      <div class="duty-item">
                        <div class="duty-label">HTS 2</div>
                        <div class="duty-val">Code: {{ part.htsCode2 || '-' }}</div>
                        <div class="duty-val">Rate: {{ part.rate2 !== null && part.rate2 !== undefined ? part.rate2 + '%' : '-' }}</div>
                      </div>
                      <div class="duty-item">
                        <div class="duty-label">HTS 3</div>
                        <div class="duty-val">Code: {{ part.htsCode3 || '-' }}</div>
                        <div class="duty-val">Rate: {{ part.rate3 !== null && part.rate3 !== undefined ? part.rate3 + '%' : '-' }}</div>
                      </div>
                      <div class="duty-item">
                        <div class="duty-label">HTS 4</div>
                        <div class="duty-val">Code: {{ part.htsCode4 || '-' }}</div>
                        <div class="duty-val">Rate: {{ part.rate4 !== null && part.rate4 !== undefined ? part.rate4 + '%' : '-' }}</div>
                      </div>
                    </div>
                  </div>
                </td>
              </tr>
            </template>
            <tr v-if="!loading && parts.length === 0">
              <td colspan="12" class="no-data text-center">{{ $t('common.no_data') }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="totalCount"
          background
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper {
  background-color: var(--dashboard-bg);
  min-height: 100vh;
}

.page-container {
  padding: 2.5rem;
  max-width: 1400px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

.page-header {
  margin-bottom: 2.5rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid rgba(0,0,0,0.05);
}

h1 {
  font-size: 2rem;
  color: var(--sidebar-color);
  margin: 0;
}

.filter-card {
  margin-bottom: 1.5rem;
  padding: 1.5rem;
  border-radius: 12px;
}

.filter-grid {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr;
  gap: 1.5rem;
}

.search-input-group {
  display: flex;
  gap: 1.5rem;
  align-items: flex-start;
}

.group-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  flex: 1;
}

.customer-select {
  width: 100%;
}

.filter-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-item label {
  font-size: 0.85rem;
  color: #8898aa;
}

.form-input, .form-select-el {
  width: 100%;
  font-family: inherit;
  outline: none;
}

.form-input {
  padding: 0.7rem;
  border: 1px solid #dee2e6;
  border-radius: 8px;
}

.form-select-el :deep(.el-input__wrapper) {
  padding: 4px 12px;
  border-radius: 8px;
  box-shadow: 0 0 0 1px #dee2e6 inset !important;
  height: 42px;
}

.form-input:focus, .form-select-el :deep(.el-input__wrapper.is-focus) {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 1px var(--primary-color) inset !important;
}

.action-row {
  margin-top: 1rem;
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.table-wrapper {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  min-width: 1200px;
}

.data-table th, .data-table td {
  padding: 1.2rem 1.5rem;
  text-align: left;
  border-bottom: 1px solid #f0f2f5;
}

.data-table th {
  background-color: #f8f9fe;
  color: #8898aa;
  font-size: 0.85rem;
  text-transform: uppercase;
}

.part-no-cell {
  font-weight: bold;
}

.part-no-cell a {
  color: var(--primary-color);
  text-decoration: none;
}

.part-no-cell a:hover {
  text-decoration: underline;
}

.desc-cell {
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.expand-toggle {
  cursor: pointer;
  color: var(--primary-color);
  font-weight: bold;
  font-size: 1.2rem;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 24px;
  height: 24px;
}

.expanded-row-master {
  background-color: #f8f9fe;
}

.detail-row td {
  padding: 0 !important;
}

.detail-content {
  background-color: #fafbfd;
  padding: 1.5rem 3rem;
  border-bottom: 1px solid #f0f2f5;
}

.duty-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 2rem;
}

.duty-label {
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--sidebar-color);
  margin-bottom: 0.5rem;
  border-bottom: 2px solid var(--primary-color);
  display: inline-block;
}

.duty-val {
  font-size: 0.85rem;
  color: #525f7f;
}

.sla-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.8rem;
  background-color: #f1f3f5;
  color: #8898aa;
}

.sla-badge.active {
  background-color: #fff1f0;
  color: #f5222d;
  border: 1px solid #ffa39e;
}

.status-cell {
  display: flex;
  align-items: center;
  gap: 0.6rem;
}

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: 2.5rem;
  padding: 1.5rem 0;
}

.time-cell {
  color: #8898aa;
  font-size: 0.9rem;
}

code {
  background: #f1f3f5;
  padding: 2px 6px;
  border-radius: 4px;
  font-family: monospace;
}
</style>
