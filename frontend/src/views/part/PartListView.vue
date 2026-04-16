<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { authService, UserRole } from '../../services/auth/auth';
import { partService, type Part, PartStatus } from '../../services/part/part';
import { commonService, type CustomerOption, type StatusOption, type SupplierOption } from '../../services/common/common';
import Card from '../../components/common/Card.vue';
import Dot from '../../components/common/Dot.vue';
import Button from '../../components/common/Button.vue';

/**
 * Part No List View (零件編號清單頁面)
 * Sidebar layout version.
 * 
 * Audit Update on 2026-04-16 by Gemini AI:
 * Ticket: UI-SEARCH-SUPPLIER-001
 * Intent: Update supplier filter to use /api/common/suppliers and reload when customer changes.
 * Impact: UI logic and dynamic data fetching.
 * (繁體中文) 2026-04-16 Gemini AI 更新：更新供應商過濾器以使用 /api/common/suppliers 並在客戶變更時重新載入。
 */

const route = useRoute();
const router = useRouter();

// INTERNAL-AI-20260416: Capture auth state immediately for filtering
const { role, customerId: authCustomerId } = authService.state;
const isEmployee = computed(() => role && role !== UserRole.CUSTOMER);
const userCustomerId = computed(() => authCustomerId);

const parts = ref<Part[]>([]);
const suppliers = ref<SupplierOption[]>([]);
const customers = ref<CustomerOption[]>([]);
const statusOptions = ref<StatusOption[]>([]);
const loading = ref(true);

// Search and Filter states
const searchQuery = ref('');
const statusFilter = ref<string>((route.query.status as string) || '');
const supplierFilter = ref('');
const customerFilter = ref<string>((route.query.customerId as string) || (isEmployee.value ? '' : userCustomerId.value || ''));
const sortBy = ref<'partNo' | 'lastUpdated'>('lastUpdated');
const sortOrder = ref<'asc' | 'desc'>('desc');

// INTERNAL-AI-20260416: Track expanded rows
const expandedRows = ref<Set<string>>(new Set());

const toggleRow = (id: string) => {
  if (expandedRows.value.has(id)) {
    expandedRows.value.delete(id);
  } else {
    expandedRows.value.add(id);
  }
};

/**
 * Format remaining minutes to a human-readable SLA countdown.
 * (將剩餘分鐘數格式化為可讀的 SLA 倒數。)
 */
const formatSLA = (deadline?: string) => {
  if (!deadline) return '-';
  const now = new Date();
  const target = new Date(deadline);
  const diff = target.getTime() - now.getTime();
  if (diff <= 0) return 'OVERDUE';
  const mins = Math.floor(diff / 60000);
  const h = Math.floor(mins / 60);
  const m = mins % 60;
  return `${h}h ${m}m`;
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
    result = result.filter(p => p.customerId === effectiveCustomerId);
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
    const selectedSupplier = suppliers.value.find(s => s.key === supplierFilter.value);
    const supplierName = selectedSupplier ? selectedSupplier.value : supplierFilter.value;
    result = result.filter(p => p.supplier === supplierName);
  }
  
  result.sort((a, b) => {
    const valA = a[sortBy.value];
    const valB = b[sortBy.value];
    if (sortOrder.value === 'asc') {
      return valA > valB ? 1 : -1;
    } else {
      return valA < valB ? 1 : -1;
    }
  });
  return result;
});

const getStatusColor = (status: PartStatus) => {
  const colors: Record<string, string> = {
    [PartStatus.UNKNOWN]: '#909399',
    [PartStatus.PENDING_CUSTOMER]: '#E6A23C',
    [PartStatus.PENDING_REVIEW]: '#409EFF',
    [PartStatus.RETURNED]: '#F56C6C',
    [PartStatus.ACTIVE]: '#67C23A',
    [PartStatus.FLAGGED]: '#E6A23C',
    [PartStatus.SUPERSEDED]: '#909399'
  };
  return colors[status] || '#909399';
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
              <th>{{ $t('part_list.division') }}</th>
              <th @click="sortBy = 'partNo'; sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'">
                {{ $t('customer.part_no') }} {{ sortBy === 'partNo' ? (sortOrder === 'asc' ? '↑' : '↓') : '' }}
              </th>
              <th>{{ $t('part_list.description') }}</th>
              <th>{{ $t('part_list.coo') }}</th>
              <th>{{ $t('customer.hts_code') }}</th>
              <th>{{ $t('part_list.duty_rate') }}</th>
              <th>{{ $t('common.status') }}</th>
              <th>{{ $t('part_list.updated_by') }}</th>
              <th @click="sortBy = 'lastUpdated'; sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'">
                {{ $t('common.last_updated') }} {{ sortBy === 'lastUpdated' ? (sortOrder === 'asc' ? '↑' : '↓') : '' }}
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
                <td>{{ part.division || '-' }}</td>
                <td class="part-no-cell">
                  <a href="javascript:void(0)" @click="router.push({ name: 'part-detail', params: { id: part.id } })">
                    {{ part.partNo }}
                  </a>
                </td>
                <td class="desc-cell">{{ part.description || '-' }}</td>
                <td>{{ part.countryOfOrigin || '-' }}</td>
                <td><code>{{ part.htsCode }}</code></td>
                <td>{{ part.generalDutyRate !== undefined ? part.generalDutyRate + '%' : '-' }}</td>
                <td>
                  <div class="status-cell">
                    <Dot :color="getStatusColor(part.status)" size="8px" />
                    <span>{{ $t('status.' + part.status.toLowerCase()) }}</span>
                  </div>
                </td>
                <td>{{ part.updatedBy || '-' }}</td>
                <td class="time-cell">{{ part.lastUpdated }}</td>
                <td>
                  <span :class="['sla-badge', part.slaDeadline ? 'active' : '']">
                    {{ formatSLA(part.slaDeadline) }}
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
                        <div class="duty-label">301 Duty</div>
                        <div class="duty-val">Code: {{ part.duty301?.code || '9903.88.01' }}</div>
                        <div class="duty-val">Rate: {{ part.duty301?.rate || '25%' }}</div>
                      </div>
                      <div class="duty-item">
                        <div class="duty-label">IEEPA Duty</div>
                        <div class="duty-val">Code: {{ part.dutyIEEPA?.code || '-' }}</div>
                        <div class="duty-val">Rate: {{ part.dutyIEEPA?.rate || '-' }}</div>
                      </div>
                      <div class="duty-item">
                        <div class="duty-label">232 Aluminum</div>
                        <div class="duty-val">Code: {{ part.duty232Aluminum?.code || '-' }}</div>
                        <div class="duty-val">Rate: {{ part.duty232Aluminum?.rate || '-' }}</div>
                      </div>
                      <div class="duty-item">
                        <div class="duty-label">Reciprocal Tariff</div>
                        <div class="duty-val">Code: {{ part.dutyReciprocal?.code || '-' }}</div>
                        <div class="duty-val">Rate: {{ part.dutyReciprocal?.rate || '-' }}</div>
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
