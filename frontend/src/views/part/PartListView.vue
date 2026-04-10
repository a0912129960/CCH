<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { partService, type Part, PartStatus } from '../../services/part/part';
import Card from '../../components/common/Card.vue';
import Dot from '../../components/common/Dot.vue';
import Button from '../../components/common/Button.vue';

/**
 * Part No List View (零件編號清單頁面)
 * Sidebar layout version.
 * 
 * Audit Update on 2026-04-09 by Gemini AI:
 * Ticket: UI-REFACTOR-001
 * Intent: Move "Add New Part" button to below search bar for better UX and apply project-standard Button style.
 * Impact: UI layout adjustment and component standardization.
 * (繁體中文) 2026-04-09 Gemini AI 更新：將「新增零件」按鈕移至搜尋列下方，並套用專案標準 Button 樣式。
 */

const route = useRoute();
const router = useRouter();

const parts = ref<Part[]>([]);
const suppliers = ref<string[]>([]);
const loading = ref(true);

// Search and Filter states
const searchQuery = ref('');
const statusFilter = ref<string>((route.query.status as string) || '');
const supplierFilter = ref('');
const sortBy = ref<'partNo' | 'lastUpdated'>('lastUpdated');
const sortOrder = ref<'asc' | 'desc'>('desc');

onMounted(async () => {
  try {
    const [partsData, suppliersData] = await Promise.all([
      partService.getParts(),
      partService.getSuppliers()
    ]);
    parts.value = partsData;
    suppliers.value = suppliersData;
  } finally {
    loading.value = false;
  }
});

const filteredParts = computed(() => {
  let result = [...parts.value];
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase();
    result = result.filter(p => 
      p.partNo.toLowerCase().includes(q) || 
      p.htsCode.toLowerCase().includes(q)
    );
  }
  if (statusFilter.value) {
    result = result.filter(p => p.status === statusFilter.value);
  }
  if (supplierFilter.value) {
    result = result.filter(p => p.supplier === supplierFilter.value);
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
          <!-- Search -->
          <div class="filter-item search">
            <label>{{ $t('common.search') }}</label>
            <input 
              v-model="searchQuery" 
              type="text" 
              :placeholder="$t('part_list.search_placeholder')"
              class="form-input"
            />
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
                v-for="s in Object.values(PartStatus)" 
                :key="s" 
                :label="$t('status.' + s.toLowerCase())" 
                :value="s" 
              />
            </el-select>
          </div>

          <!-- Supplier Filter -->
          <div class="filter-item">
            <label>{{ $t('part_list.filter_supplier') }}</label>
            <el-select v-model="supplierFilter" class="form-select-el" clearable filterable>
              <el-option :label="$t('common.all')" value="" />
              <el-option v-for="s in suppliers" :key="s" :label="s" :value="s" />
            </el-select>
          </div>
        </div>
      </Card>

      <div class="table-wrapper">
        <table class="data-table">
          <thead>
            <tr>
              <th @click="sortBy = 'partNo'; sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'">
                {{ $t('customer.part_no') }} {{ sortBy === 'partNo' ? (sortOrder === 'asc' ? '↑' : '↓') : '' }}
              </th>
              <th>{{ $t('customer.hts_code') }}</th>
              <th>{{ $t('common.supplier') }}</th>
              <th>{{ $t('common.status') }}</th>
              <th @click="sortBy = 'lastUpdated'; sortOrder = sortOrder === 'asc' ? 'desc' : 'asc'">
                {{ $t('common.last_updated') }} {{ sortBy === 'lastUpdated' ? (sortOrder === 'asc' ? '↑' : '↓') : '' }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading"><td colspan="5" class="text-center">Loading...</td></tr>
            <tr v-else v-for="part in filteredParts" :key="part.id">
              <td class="part-no-cell">
                <a href="javascript:void(0)" @click="router.push({ name: 'part-detail', params: { id: part.id } })">
                  {{ part.partNo }}
                </a>
              </td>
              <td><code>{{ part.htsCode }}</code></td>
              <td>{{ part.supplier }}</td>
              <td>
                <div class="status-cell">
                  <Dot :color="getStatusColor(part.status)" size="8px" />
                  <span>{{ $t('status.' + part.status.toLowerCase()) }}</span>
                </div>
              </td>
              <td class="time-cell">{{ part.lastUpdated }}</td>
            </tr>
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
  overflow: hidden;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
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
