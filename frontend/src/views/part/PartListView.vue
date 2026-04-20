<script setup lang="ts">
import { authService, UserRole } from '@src/services/auth/auth';
import { partService, batchAcceptParts, type PartListItem } from '@src/services/part/part';
import { commonService, type CustomerOption, type StatusOption, type SupplierOption } from '@src/services/common/common';

// Internal components
import Card from '@src/components/common/Card.vue';
import Dot from '@src/components/common/Dot.vue';
import Button from '@src/components/common/Button.vue';

import { CaretRight, CaretBottom } from '@element-plus/icons-vue';

/**
 * Part No List View (零件編號清單頁面)
 * Audit Update on 2026-04-17: Final precision lint & SonarQube fixes.
 * Update by Gemini AI: Shortened imports, fixed attributes, and resolved contrast/unused CSS.
 */

const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const role = computed(() => authService.state.role);
const authCustomerId = computed(() => authService.state.customerId);
const isEmployee = computed(() => role.value && role.value !== UserRole.CUSTOMER);
const isDcb = computed(() => role.value === UserRole.DCB);
const userCustomerId = computed(() => authCustomerId.value);

/**
 * Update by Gemini AI on 2026-04-18: Fixed reactive role detection to ensure feature visibility (Batch Accept) and verified export function. (修復響應式角色偵測以確保功能顯示，並驗證匯出功能。)
 */

// State Management
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

// Selection Logic for Batch Accept (批次接受的選擇邏輯)
const selectedIds = ref<Set<number>>(new Set());
const selectableParts = computed(() => 
  parts.value.filter(p => p.status === 'S02' || p.status === 'S03')
);

const isAllSelectableSelected = computed(() => 
  selectableParts.value.length > 0 && selectableParts.value.every(p => selectedIds.value.has(p.id))
);

const toggleSelection = (id: number) => {
  if (selectedIds.value.has(id)) {
    selectedIds.value.delete(id);
  } else {
    selectedIds.value.add(id);
  }
};

const toggleAllSelection = () => {
  if (isAllSelectableSelected.value) {
    selectableParts.value.forEach(p => selectedIds.value.delete(p.id));
  } else {
    selectableParts.value.forEach(p => selectedIds.value.add(p.id));
  }
};

const handleBatchAccept = async () => {
  if (selectedIds.value.size === 0) return;

  try {
    await ElMessageBox.confirm(
      `${t('part_detail.accept_confirm')} (${selectedIds.value.size} items)`,
        t('part_list.batch_accept'),
      { confirmButtonClass: 'btn-confirm-orange', type: 'warning' }
    );

    loading.value = true;
    const idsToAccept = Array.from(selectedIds.value).map(id => Number(id));

    // Call batch API (呼叫批量接受 API)
    const result = await batchAcceptParts(idsToAccept);
    
    if (result.success) {
      ElMessage.success(result.message || 'Batch accept successful');
      selectedIds.value.clear();
      await fetchParts();
    } else {
      ElMessage.error(result.message || 'Batch accept failed');
      // Handle partial failures (處理部分失敗)
      if (result.data && result.data.length > 0) {
        console.error('Partial failures:', result.data);
      }
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error(error);
      ElMessage.error('Batch accept failed. (批次接受失敗。)');
    }
  } finally {
    loading.value = false;
  }
};

// Constants & Mappings
const HTS_LABELS = [
  '301 Duty',
  'IEEPA Duty',
  '232 Aluminum',
  'Reciprocal Tariff'
] as const;

const SLA_COLOR_MAP: Record<string, string> = {
  'green': '#67C23A',
  'normal': '#67C23A',
  'yellow': '#FADB14',
  'orange': '#FF9900',
  'warning': '#FF9900',
  'red': '#F56C6C',
  'urgent': '#F56C6C'
};

// Handlers
const toggleRow = (id: number) => {
  if (expandedRows.value.has(id)) {
    expandedRows.value.delete(id);
  } else {
    expandedRows.value.add(id);
  }
};

const isAllExpanded = computed(() => (
  parts.value.length > 0 && parts.value.every(p => expandedRows.value.has(p.id))
));

const toggleAll = () => {
  if (isAllExpanded.value) {
    expandedRows.value.clear();
  } else {
    parts.value.forEach(p => expandedRows.value.add(p.id));
  }
};

const exportToExcel = async () => {
  try {
    ElMessage.success('Exporting to Excel... (正在匯出至 Excel...)');
    await partService.exportPartsToExcel({
      customerId: customerFilter.value || undefined,
      status: statusFilter.value || undefined,
      partNo: searchQuery.value || undefined,
      supplier: supplierFilter.value || undefined
    });
  } catch (error) {
    console.error('Export failed (匯出失敗):', error);
    ElMessage.error('Export failed. (匯出失敗。)');
  }
};

const getSLAColor = (slaStatus?: string) => {
  if (!slaStatus) return 'transparent';
  /* return SLA_COLOR_MAP[slaStatus.toLowerCase()] || '#909399'; */
  return SLA_COLOR_MAP[slaStatus.toLowerCase()] || 'transparent';
};

/**
 * Data Fetching Logic
 */
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
  } catch {
    // Silent fail
  } finally {
    loading.value = false;
  }
};

// Lifecycle & Watchers
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
  } catch {
    loading.value = false;
  }
});

let searchTimeout: ReturnType<typeof setTimeout> | null = null;

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
  searchTimeout = setTimeout(fetchParts, 1000);
});

watch([currentPage, pageSize], fetchParts);
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
                <label for="customer-select">{{ $t('employee.customer_select') }}</label>
                <el-select 
                  id="customer-select"
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
                <label for="search-input">{{ $t('common.search') }}</label>
                <input 
                  id="search-input"
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
              <Button class="ml-4" @click="router.push({ name: 'part-upload' })">
                {{ $t('part_list.bulk_upload') }}
              </Button>
              <Button class="ml-4" @click="exportToExcel">
                {{ $t('part_list.export_excel') }}
              </Button>
              <Button v-if="isDcb" class="ml-4 btn-batch-accept" :disabled="selectedIds.size === 0" @click="handleBatchAccept">
                {{ $t('part_list.batch_accept') }}
              </Button>
            </div>
          </div>
          <div class="filter-item">
            <label for="status-filter">{{ $t('part_list.filter_status') }}</label>
            <el-select id="status-filter" v-model="statusFilter" class="form-select-el" clearable>
              <el-option :label="$t('common.all')" value="" />
              <el-option v-for="s in statusOptions" :key="s.key" :label="s.value" :value="s.key" />
            </el-select>
          </div>
          <div class="filter-item">
            <label for="supplier-filter">{{ $t('part_list.filter_supplier') }}</label>
            <el-select id="supplier-filter" v-model="supplierFilter" class="form-select-el" clearable filterable>
              <el-option :label="$t('common.all')" value="" />
              <el-option v-for="s in suppliers" :key="s.key" :label="s.value" :value="s.key" />
            </el-select>
          </div>
        </div>
      </Card>

      <!-- SLA Rule Legend (SLA 規則圖例) - Moved between Filter and Grid (移至篩選與網格之間) -->
      <div class="sla-legend-row">
        <span class="legend-desc">
          {{ isEmployee ? $t('part_list.sla_legend.staff_rule') : $t('part_list.sla_legend.customer_rule') }}
        </span>
        <div class="legend-items">
          <div class="legend-item">
            <Dot dotColor="#67C23A" size="8px" />
            <span class="label">{{ $t('part_list.sla_legend.range_normal') }}:</span>
            <span class="val">{{ isEmployee ? $t('part_list.sla_legend.staff_normal') : $t('part_list.sla_legend.customer_normal') }}</span>
          </div>
          <div class="legend-item">
            <Dot dotColor="#FADB14" size="8px" />
            <span class="label">{{ $t('part_list.sla_legend.range_warning') }}:</span>
            <span class="val">{{ isEmployee ? $t('part_list.sla_legend.staff_warning') : $t('part_list.sla_legend.customer_warning') }}</span>
          </div>
          <div class="legend-item">
            <Dot dotColor="#FF9900" size="8px" />
            <span class="label">{{ $t('part_list.sla_legend.range_urgent') }}:</span>
            <span class="val">{{ isEmployee ? $t('part_list.sla_legend.staff_urgent') : $t('part_list.sla_legend.customer_urgent') }}</span>
          </div>
          <div class="legend-item">
            <Dot dotColor="#F56C6C" size="8px" />
            <span class="label">{{ $t('part_list.sla_legend.range_overdue') }}:</span>
            <span class="val">{{ isEmployee ? $t('part_list.sla_legend.staff_overdue') : $t('part_list.sla_legend.customer_overdue') }}</span>
          </div>
        </div>
      </div>

      <div class="table-wrapper">
        <table class="data-table">
          <thead>
            <tr>
              <th scope="col" class="text-center col-expand">
                <button 
                  type="button"
                  class="expand-toggle" 
                  :title="$t('common.collapse_all')"
                  @click="toggleAll"
                >
                  <el-icon v-if="isAllExpanded"><CaretBottom /></el-icon>
                  <el-icon v-else><CaretRight /></el-icon>
                </button>
              </th>
              <th v-if="isDcb" scope="col" class="text-center col-checkbox">
                <el-checkbox 
                  :model-value="isAllSelectableSelected" 
                  :indeterminate="selectedIds.size > 0 && !isAllSelectableSelected"
                  @change="toggleAllSelection"
                />
              </th>
              <th scope="col" class="col-customer">{{ $t('common.customer') }}</th>
              <th scope="col" class="col-partno">{{ $t('customer.part_no') }}</th>
              <th scope="col" class="col-desc">{{ $t('part_list.description') }}</th>
              <th scope="col" class="col-country">{{ $t('part_detail.country') }}</th>
              <th scope="col" class="col-hts">{{ $t('customer.hts_code') }}</th>
              <th scope="col" class="wrap-header col-rate">{{ $t('part_list.duty_rate') }}</th>
              <th scope="col" class="col-status">{{ $t('common.status') }}</th>
              <th scope="col" class="col-updatedby">{{ $t('part_list.updated_by') }}</th>
              <th scope="col" class="col-updateddate">{{ $t('common.last_updated') }}</th>
              <th scope="col" class="col-sla">{{ $t('part_list.sla') }}</th>
              <th scope="col" class="col-actions">{{ $t('common.actions') }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td :colspan="isDcb ? 13 : 12" class="text-center">Loading...</td>
            </tr>
            <template v-else v-for="part in parts" :key="part.id">
              <tr :class="{ 'expanded-row-master': expandedRows.has(part.id) }">
                <td class="text-center col-expand">
                  <button 
                    type="button"
                    class="expand-toggle" 
                    @click="toggleRow(part.id)"
                  >
                    <el-icon v-if="expandedRows.has(part.id)"><CaretBottom /></el-icon>
                    <el-icon v-else><CaretRight /></el-icon>
                  </button>
                </td>
                <td v-if="isDcb" class="text-center col-checkbox">
                  <el-checkbox 
                    v-if="part.status === 'S02' || part.status === 'S03'"
                    :model-value="selectedIds.has(part.id)" 
                    @change="toggleSelection(part.id)"
                  />
                </td>
                <td class="col-customer" :title="part.customer">{{ part.customer || '-' }}</td>
                <td class="col-partno" :title="part.partNo">{{ part.partNo }}</td>
                <td class="col-desc" :title="part.partDesc">{{ part.partDesc || '-' }}</td>
                <td class="col-country" :title="part.country">{{ part.country || '-' }}</td>
                <td class="col-hts"><code>{{ part.htsCode }}</code></td>
                <td class="col-rate">{{ part.rate !== undefined ? part.rate + '%' : '-' }}</td>
                <td class="col-status">{{ $t('status.' + part.status.toLowerCase()) }}</td>
                <td class="col-updatedby" :title="part.updatedBy">{{ part.updatedBy || '-' }}</td>
                <td class="col-updateddate">{{ commonService.formatDateTime(part.updatedDate) }}</td>
                <td class="col-sla">
                  <div v-if="part.slaStatus && getSLAColor(part.slaStatus) !== 'transparent'" class="status-cell">
                    <Dot :dotColor="getSLAColor(part.slaStatus)" size="8px" />
                  </div>
                </td>
                <td class="col-actions">
                  <Button class="btn-compact" @click="router.push({ name: 'part-detail', params: { id: part.id } })">
                    View
                  </Button>
                </td>
              </tr>
              <tr v-if="expandedRows.has(part.id)" class="detail-row">
                <td :colspan="isDcb ? 13 : 12">
                  <div class="detail-content">
                    <div class="duty-grid">
                      <div class="duty-item" v-for="i in 4" :key="i">
                        <div class="duty-label">{{ HTS_LABELS[i-1] }}</div>
                        <div class="duty-val">Code: {{ part[`htsCode${i as 1|2|3|4}`] || '-' }}</div>
                        <div class="duty-val">Rate: {{ part[`rate${i as 1|2|3|4}`] ?? '-' }}{{ part[`rate${i as 1|2|3|4}`] ? '%' : '' }}</div>
                      </div>
                    </div>
                  </div>
                </td>
              </tr>
            </template>
            <tr v-if="!loading && parts.length === 0">
              <td :colspan="isDcb ? 13 : 12" class="no-data text-center">{{ $t('common.no_data') }}</td>
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

.sla-legend-row {
  margin-top: 0.8rem;
  padding: 0.5rem 0.8rem;
  background-color: #f8f9fe;
  border-radius: 6px;
}

.legend-desc {
  display: block;
  font-size: 0.75rem;
  color: #525f7f;
  margin-bottom: 0.4rem;
  font-weight: 600;
}

.legend-items {
  display: flex;
  flex-wrap: wrap;
  gap: 1.2rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.4rem;
}

.legend-item .label {
  font-size: 0.7rem;
  color: #8898aa;
  font-weight: 600;
}

.legend-item .val {
  font-size: 0.75rem;
  color: var(--sidebar-color);
  font-weight: 700;
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
  color: #525f7f;
}

.form-input {
  padding: 0.5rem;
  border: 1px solid #dee2e6;
  border-radius: 6px;
}

.action-row {
  margin-top: 0.8rem;
  display: flex;
  align-items: center;
  gap: 1rem; /* Allow buttons to wrap to next line if needed, instead of squeezing text (允許按鈕在空間不足時整顆折行，而非壓縮文字) */
}

.action-row :deep(.btn-cch), 
.action-row :deep(button) {
  white-space: nowrap; /* Prevent text inside button from wrapping (防止按鈕內文字折行) */
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
  table-layout: fixed;
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
  font-size: 0.85rem;
}

.data-table th {
  background-color: #f8f9fe;
  color: #8898aa;
  text-transform: none;
  font-weight: 600;
}

/* Modern Column Width Control */
.col-expand { width: 35px; }
.col-checkbox { width: 40px; }
.col-customer { width: 100px; }
.col-partno { width: 80px; }
.col-desc { width: 120px; }
.col-country { width: 60px; }
.col-hts { width: 90px; }
.col-rate { width: 50px; }
.col-status { width: 120px; }
.col-updatedby { width: 80px; }
.col-updateddate { width: 100px; }
.col-sla { width: 40px; }
.col-actions { width: 65px; }

.wrap-header {
  white-space: normal !important;
  line-height: 1.0 !important;
  word-break: break-all !important;
  max-width: 55px !important;
  min-width: 55px !important;
  padding: 0.2rem 1px !important;
}

.btn-compact {
  padding: 2px 4px !important;
  font-size: 0.7rem !important;
  height: 20px !important;
  line-height: 1 !important;
}

.btn-batch-accept {
  background-color: var(--primary-color) !important;
  border-color: var(--primary-color) !important;
  color: white !important;
}

.btn-batch-accept:disabled {
  background-color: #a0cfff !important;
  border-color: #a0cfff !important;
  cursor: not-allowed;
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
  background: none;
  border: none;
  padding: 0;
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

.ml-4 { margin-left: 1rem; }
</style>
