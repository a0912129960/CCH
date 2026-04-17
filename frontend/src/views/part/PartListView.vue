<script setup lang="ts">
import { authService, UserRole } from '../../services/auth/auth';
import { partService, type PartListItem } from '../../services/part/part';
import { commonService, type CustomerOption, type StatusOption, type SupplierOption } from '../../services/common/common';

// Internal components (Restore manual import for Vitest compatibility)
import Card from '../../components/common/Card.vue';
import Dot from '../../components/common/Dot.vue';
import Button from '../../components/common/Button.vue';

import { CaretRight, CaretBottom, Upload } from '@element-plus/icons-vue';

/**
 * Part No List View (零件編號清單頁面)
 * Audit Update on 2026-04-17: Relocate Toggle All to Grid Header.
 * Update by Gemini AI: Changed icons to small arrows (Gray) and moved Toggle All to First Column.
 */

const route = useRoute();
const router = useRouter();

const { role, customerId: authCustomerId } = authService.state;
const isEmployee = computed(() => role && role !== UserRole.CUSTOMER);
const userCustomerId = computed(() => authCustomerId);

const parts = ref<PartListItem[]>([]);
const suppliers = ref<SupplierOption[]>([]);
const customers = ref<CustomerOption[]>([]);
const statusOptions = ref<StatusOption[]>([]);
const loading = ref(true);

const currentPage = ref(1);
const pageSize = ref(10);
const totalCount = ref(0);

const searchQuery = ref('');
const statusFilter = ref<string>((route.query.status as string) || '');
const supplierFilter = ref('');
const customerFilter = ref<string>((route.query.customerId as string) || (isEmployee.value ? '' : userCustomerId.value || ''));

const expandedRows = ref<Set<number>>(new Set());

const toggleRow = (id: number) => {
  if (expandedRows.value.has(id)) {
    expandedRows.value.delete(id);
  } else {
    expandedRows.value.add(id);
  }
};

/**
 * Check if all visible rows are expanded (檢查是否所有可見行皆已展開)
 */
const isAllExpanded = computed(() => {
  return parts.value.length > 0 && parts.value.every(p => expandedRows.value.has(p.id));
});

/**
 * Toggle all rows (切換所有行的展開狀態)
 */
const toggleAll = () => {
  if (isAllExpanded.value) {
    expandedRows.value.clear();
  } else {
    parts.value.forEach(p => expandedRows.value.add(p.id));
  }
};

const htsLabels = [
  '301 Duty',
  'IEEPA Duty',
  '232 Aluminum',
  'Reciprocal Tariff'
];

/**
 * Helper to get HTS fields safely from PartListItem (輔助函式：安全獲取 HTS 欄位)
 */
const getHTSCode = (part: PartListItem, i: number): string => {
  const key = `htsCode${i}` as keyof PartListItem;
  return (part[key] as string) || '-';
};

const getHTSRate = (part: PartListItem, i: number): string => {
  const key = `rate${i}` as keyof PartListItem;
  const val = part[key];
  return val !== null && val !== undefined ? val + '%' : '-';
};

const reloadSuppliers = async () => {
  const cId = customerFilter.value || 'all';
  suppliers.value = await commonService.getSuppliers(cId);
  if (supplierFilter.value && !suppliers.value.some(s => s.key === supplierFilter.value)) {
    supplierFilter.value = '';
  }
};

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

let searchTimeout: any = null;
watch(customerFilter, async () => {
  currentPage.value = 1;
  await reloadSuppliers();
  await fetchParts();
});

watch([statusFilter, supplierFilter], async () => {
  currentPage.value = 1;
  await fetchParts();
});

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

const getSLAColor = (slaStatus?: string) => {
  if (!slaStatus) return 'transparent';
  const s = slaStatus.toLowerCase();
  const colors: Record<string, string> = {
    'green': '#67C23A',
    'normal': '#67C23A',
    'yellow': '#FADB14',
    'orange': '#FF9900',
    'warning': '#FF9900',
    'red': '#F56C6C',
    'urgent': '#F56C6C'
  };
  return colors[s] || '#909399';
};
</script>

<template>
  <div class="page-wrapper">
    <div class="page-container">
      <header class="page-header">
        <div class="header-title-row">
          <h1>{{ $t('part_list.title') }}</h1>
        </div>
      </header>

      <Card class="filter-card">
        <div class="filter-grid">
          <div class="filter-item search">
            <div class="search-input-group">
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
                <el-icon><Upload /></el-icon> {{ $t('part_list.bulk_upload') }}
              </Button>
            </div>
          </div>
          <div class="filter-item">
            <label>{{ $t('part_list.filter_status') }}</label>
            <el-select v-model="statusFilter" class="form-select-el" clearable>
              <el-option :label="$t('common.all')" value="" />
              <el-option v-for="s in statusOptions" :key="s.key" :label="s.value" :value="s.key" />
            </el-select>
          </div>
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
              <th width="35" class="text-center">
                <span class="expand-toggle" @click="toggleAll" :title="$t('common.collapse_all')">
                  <el-icon v-if="isAllExpanded"><CaretBottom /></el-icon>
                  <el-icon v-else><CaretRight /></el-icon>
                </span>
              </th>
              <th width="100">{{ $t('common.customer') }}</th>
              <th width="80">{{ $t('customer.part_no') }}</th>
              <th width="120">{{ $t('part_list.description') }}</th>
              <th width="60">{{ $t('part_detail.country') }}</th>
              <th width="90">{{ $t('customer.hts_code') }}</th>
              <th width="50" class="wrap-header">{{ $t('part_list.duty_rate') }}</th>
              <th width="120">{{ $t('common.status') }}</th>
              <th width="80">{{ $t('part_list.updated_by') }}</th>
              <th width="100">{{ $t('common.last_updated') }}</th>
              <th width="40">{{ $t('part_list.sla') }}</th>
              <th width="65">{{ $t('common.actions') }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="12" class="text-center">Loading...</td>
            </tr>
            <template v-else v-for="part in parts" :key="part.id">
              <tr :class="{ 'expanded-row-master': expandedRows.has(part.id) }">
                <td class="text-center">
                  <span class="expand-toggle" @click="toggleRow(part.id)">
                    <el-icon v-if="expandedRows.has(part.id)"><CaretBottom /></el-icon>
                    <el-icon v-else><CaretRight /></el-icon>
                  </span>
                </td>
                <td :title="part.customer">{{ part.customer || '-' }}</td>
                <td :title="part.partNo">{{ part.partNo }}</td>
                <td :title="part.partDesc">{{ part.partDesc || '-' }}</td>
                <td :title="part.country">{{ part.country || '-' }}</td>
                <td><code>{{ part.htsCode }}</code></td>
                <td>{{ part.rate !== undefined ? part.rate + '%' : '-' }}</td>
                <td>{{ $t('status.' + part.status.toLowerCase()) }}</td>
                <td :title="part.updatedBy">{{ part.updatedBy || '-' }}</td>
                <td>{{ commonService.formatDateTime(part.updatedDate) }}</td>
                <td>
                  <div v-if="part.slaStatus" class="status-cell">
                    <Dot :dotColor="getSLAColor(part.slaStatus)" size="8px" />
                  </div>
                  <span v-else>-</span>
                </td>
                <td>
                  <Button type="text" class="btn-compact" @click="router.push({ name: 'part-detail', params: { id: part.id } })">
                    View
                  </Button>
                </td>
              </tr>
              <tr v-if="expandedRows.has(part.id)" class="detail-row">
                <td colspan="12">
                  <div class="detail-content">
                    <div class="duty-grid">
                      <div class="duty-item" v-for="i in 4" :key="i">
                        <div class="duty-label">{{ htsLabels[i-1] }}</div>
                        <div class="duty-val">Code: {{ getHTSCode(part, i) }}</div>
                        <div class="duty-val">Rate: {{ getHTSRate(part, i) }}</div>
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
  padding: 1.5rem 24px;
  max-width: 100%;
  margin: 0;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

.page-header {
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
}

.header-title-row {
  display: flex;
  align-items: center;
}

h1 {
  font-size: 1.5rem;
  color: var(--sidebar-color);
  margin: 0;
}

.filter-card {
  margin-bottom: 1rem;
  padding: 1rem;
  border-radius: 12px;
}

.filter-grid {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr;
  gap: 1rem;
}

.search-input-group {
  display: flex;
  gap: 1rem;
  align-items: flex-start;
}

.group-item {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
  flex: 1;
}

.filter-item label {
  font-size: 0.75rem;
  color: #8898aa;
}

.form-input {
  padding: 0.5rem;
  border: 1px solid #dee2e6;
  border-radius: 6px;
}

.form-select-el :deep(.el-input__wrapper) {
  border-radius: 6px;
  height: 36px;
}

.action-row {
  margin-top: 0.8rem;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.table-wrapper {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  table-layout: fixed; /* Mandate fixed layout for width control (強制固定佈局以控制寬度) */
  min-width: 1100px;
}

.data-table th, 
.data-table td {
  padding: 0.2rem 0.3rem;
  text-align: left;
  border-bottom: 1px solid #f0f2f5;
  vertical-align: middle;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 0.85rem; /* Explicitly sync both (明確同步兩者) */
}

.data-table th {
  background-color: #f8f9fe;
  color: #8898aa;
  text-transform: none;
  font-weight: 600;
}

.wrap-header {
  white-space: normal !important;
  line-height: 1.0 !important;
  word-break: break-all !important; /* Force break continuous text (強制斷開連續文字) */
  max-width: 55px !important; /* Narrower to force wrapping (更窄以強制折行) */
  min-width: 55px !important;
  padding: 0.2rem 1px !important;
}

.btn-compact {
  padding: 2px 4px !important;
  font-size: 0.7rem !important;
  height: 20px !important;
  line-height: 1 !important;
}

.expand-toggle {
  cursor: pointer;
  color: #909399; /* Gray (灰色) */
  display: inline-flex;
  justify-content: center;
  align-items: center;
  transition: color 0.2s ease;
  width: 20px;
  height: 20px;
}

.expand-toggle:hover {
  color: var(--primary-color);
}

.text-center {
  text-align: center !important;
}

.detail-content {
  background-color: #fafbfd;
  padding: 1rem 2rem;
}

.duty-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.duty-label {
  font-weight: 600;
  font-size: 0.8rem;
  color: var(--sidebar-color);
  border-bottom: 2px solid var(--primary-color);
  margin-bottom: 0.3rem;
  display: inline-block;
}

.duty-val {
  font-size: 0.75rem;
}

.status-cell {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  justify-content: center;
}

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: 1.5rem;
  padding: 1rem 0;
}

code {
  background: #f1f3f5;
  padding: 2px 4px;
  border-radius: 4px;
  font-family: monospace;
  font-size: 0.8rem;
}

.mr-1 { margin-right: 0.25rem; }
.ml-4 { margin-left: 1rem; }
</style>
