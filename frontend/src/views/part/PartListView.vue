<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { authService, UserRole } from '../../services/auth/auth';
import { partService, type PartListItem } from '../../services/part/part';
import { commonService, type CustomerOption, type StatusOption, type SupplierOption } from '../../services/common/common';
import { useI18n } from 'vue-i18n';
import Card from '../../components/common/Card.vue';
import Dot from '../../components/common/Dot.vue';
import Button from '../../components/common/Button.vue';

/**
 * Part No List View (零件編號清單頁面)
 * Sidebar layout version.
 * 
 * Audit Update on 2026-04-17 by Gemini AI:
 * Ticket: INTERNAL-AI-20260417
 * Intent: Re-connect to the latest backend API response structure for parts list.
 * Impact: UI data binding, sorting, and status/SLA display.
 * (繁體中文) 2026-04-17 Gemini AI 更新：重新串接最新的零件清單後端 API 回應結構。
 */

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

// INTERNAL-AI-20260416: Capture auth state immediately for filtering
const { role, customerId: authCustomerId } = authService.state;
const isEmployee = computed(() => role && role !== UserRole.CUSTOMER);
const userCustomerId = computed(() => authCustomerId);

const parts = ref<PartListItem[]>([]);
const suppliers = ref<SupplierOption[]>([]);
const customers = ref<CustomerOption[]>([]);
const statusOptions = ref<StatusOption[]>([]);
const loading = ref(true);

// Search and Filter states
const searchQuery = ref('');
const statusFilter = ref<string>((route.query.status as string) || '');
const supplierFilter = ref('');
const customerFilter = ref<string>((route.query.customerId as string) || (isEmployee.value ? '' : userCustomerId.value || ''));
const sortBy = ref<'partNo' | 'updatedDate'>('updatedDate');
const sortOrder = ref<'asc' | 'desc'>('desc');

// INTERNAL-AI-20260417: Track expanded rows (using number ID now)
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
  // Check if it's a color or a countdown. The new API returns green/yellow/red.
  // (繁體中文) 檢查是顏色還是倒數。新的 API 回傳 green/yellow/red。
  return t('sla.' + slaStatus.toLowerCase());
};

/**
 * Reload suppliers based on selected customer (根據所選客戶重新載入供應商)
 */
const reloadSuppliers = async () => {
  const cId = customerFilter.value || 'all';
  suppliers.value = await commonService.getSuppliers(cId);
  // Reset supplier filter if it's no longer in the list (如果不在清單中，重設供應商過濾器)
  if (supplierFilter.value && !suppliers.value.some(s => s.key === supplierFilter.value)) {
    supplierFilter.value = '';
  }
};

onMounted(async () => {
  try {
    const [partsData, customersData, statusesData] = await Promise.all([
      partService.getParts(),
      commonService.getCustomers(),
      commonService.getStatusOptions()
    ]);
    parts.value = partsData;
    customers.value = customersData;
    statusOptions.value = statusesData;

    // Initial load of suppliers (供應商初始載入)
    await reloadSuppliers();

    // Reset customer filter if not employee and fixed to user's customer
    if (!isEmployee.value && userCustomerId.value) {
      customerFilter.value = userCustomerId.value;
    }
  } finally {
    loading.value = false;
  }
});

// Watch for customer filter changes to reload suppliers (監聽客戶過濾器變更以重新載入供應商)
watch(customerFilter, async () => {
  await reloadSuppliers();
});

const filteredParts = computed(() => {
  if (!parts.value || parts.value.length === 0) return [];
  let result = [...parts.value];
  
  // INTERNAL-AI-20260416: Use customerFilter if set, otherwise fallback to user's restricted access
  const effectiveCustomerId = customerFilter.value || (!isEmployee.value ? userCustomerId.value : '');

  if (effectiveCustomerId) {
    // INTERNAL-AI-20260417: Match against part.customer in new structure
    // (繁體中文) 在新結構中與 part.customer 比對。
    result = result.filter(p => p.customer === effectiveCustomerId || customers.value.find(c => c.key === effectiveCustomerId)?.value === p.customer);
  }

  if (searchQuery.value && searchQuery.value.trim() !== '') {
    const q = searchQuery.value.toLowerCase().trim();
    result = result.filter(p => 
      (p.partNo && p.partNo.toLowerCase().includes(q)) || 
      (p.htsCode && p.htsCode.toLowerCase().includes(q))
    );
  }
  if (statusFilter.value) {
    result = result.filter(p => p.status === statusFilter.value);
  }
  if (supplierFilter.value) {
    // INTERNAL-AI-20260416: Map to supplier value/name if key is selected
    // Note: Supplier field is currently not in PartListItem, but might be part of customer or description.
    // (繁體中文) 注意：PartListItem 目前沒有供應商欄位。
  }
  
  result.sort((a, b) => {
    const valA = (a[sortBy.value] as string) || '';
    const valB = (b[sortBy.value] as string) || '';
    if (sortOrder.value === 'asc') {
      return valA > valB ? 1 : -1;
    } else {
      return valA < valB ? 1 : -1;
    }
  });
  return result;
});

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
              <!-- Customer Filter - Always visible, removed role restriction -->
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
              <Button @click="router.push({ name: 'part-create' })">
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
              <th @click="sortBy = 'partNo'; sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'">
                {{ $t('customer.part_no') }} {{ sortBy === 'partNo' ? (sortOrder === 'asc' ? '↑' : '↓') : '' }}
              </th>
              <th>{{ $t('part_list.description') }}</th>
              <th>{{ $t('part_detail.country') }}</th>
              <th>{{ $t('customer.hts_code') }}</th>
              <th>{{ $t('part_list.duty_rate') }}</th>
              <th>{{ $t('common.status') }}</th>
              <th>{{ $t('part_list.updated_by') }}</th>
              <th @click="sortBy = 'updatedDate'; sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'">
                {{ $t('common.last_updated') }} {{ sortBy === 'updatedDate' ? (sortOrder === 'asc' ? '↑' : '↓') : '' }}
              </th>
              <th>{{ $t('part_list.sla') }}</th>
              <th>{{ $t('common.actions') }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading"><td colspan="12" class="text-center">Loading...</td></tr>
            <template v-else v-for="part in filteredParts" :key="part.id">
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
          </tbody>
        </table>
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

/* Table Styles */
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
  cursor: pointer;
  user-select: none;
}

.part-no-cell {
  font-weight: bold;
}

.part-no-cell a {
  color: var(--primary-color);
  text-decoration: none;
  transition: color 0.2s;
}

.part-no-cell a:hover {
  text-decoration: underline;
  color: #0086b3;
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
  border-radius: 4px;
}

.expand-toggle:hover {
  background-color: #f0f7ff;
}

.expanded-row-master {
  background-color: #f8f9fe;
}

.detail-row td {
  padding: 0 !important;
  border-bottom: none !important;
}

.detail-content {
  background-color: #fafbfd;
  padding: 1.5rem 3rem;
  border-bottom: 1px solid #f0f2f5;
  box-shadow: inset 0 2px 4px rgba(0,0,0,0.02);
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
  margin-top: 0.2rem;
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
