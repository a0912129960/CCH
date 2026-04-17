<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { authService, UserRole } from '../../services/auth/auth';
import { partService, PartStatus } from '../../services/part/part';
import Card from '../../components/common/Card.vue';
import Button from '../../components/common/Button.vue';
import { ElMessage } from 'element-plus';

/**
 * Part No Creation View (新增零件編號頁面)
 * BR-08: HTS Format Validation | BR-21: Description Quality Scoring
 * Updated: Mandatory Customer ID for Employees and Auto-Activation logic.
 */

const router = useRouter();
const { t } = useI18n();

const { role, customerId: userCustomerId } = authService.state;
const isEmployee = role && role !== UserRole.CUSTOMER;

const form = ref({
  partNo: '',
  description: '',
  htsCode: '',
  supplier: '',
  customerId: isEmployee ? '' : userCustomerId || ''
});

const suppliers = ref<string[]>([]);
const customers = ref<{ id: string; name: string }[]>([]);

onMounted(async () => {
  try {
    const [suppliersData, customersData] = await Promise.all([
      partService.getSuppliers(),
      partService.getCustomers()
    ]);
    suppliers.value = suppliersData;
    customers.value = customersData;
  } catch (error) {
    console.error('Failed to load data:', error);
  }
});

watch(() => form.value.htsCode, (newVal) => {
  if (!newVal) return;
  let digits = newVal.replace(/\D/g, '');
  if (digits.length > 10) {
    digits = digits.slice(0, 10);
  }
  let formatted = '';
  if (digits.length > 0) {
    formatted += digits.substring(0, 4);
    if (digits.length > 4) {
      formatted += '.' + digits.substring(4, 6);
      if (digits.length > 6) {
        formatted += '.' + digits.substring(6, 10);
      }
    }
  }
  if (newVal !== formatted) {
    form.value.htsCode = formatted;
  }
});

const errors = ref({
  partNo: '',
  description: '',
  htsCode: '',
  supplier: '',
  customerId: ''
});

const htsRegex = /^\d{4}\.\d{2}\.\d{4}$/;
const isHtsValid = computed(() => !form.value.htsCode || htsRegex.test(form.value.htsCode));

const validateForm = () => {
  let valid = true;
  errors.value = { partNo: '', description: '', htsCode: '', supplier: '', customerId: '' };

  if (!form.value.partNo) {
    errors.value.partNo = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.description) {
    errors.value.description = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.supplier) {
    errors.value.supplier = 'part_create.validation.required';
    valid = false;
  }
  if (isEmployee && !form.value.customerId) {
    errors.value.customerId = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.htsCode) {
    errors.value.htsCode = 'part_create.validation.required';
    valid = false;
  } else if (!isHtsValid.value) {
    errors.value.htsCode = 'part_create.validation.hts_format';
    valid = false;
  }

  return valid;
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  try {
    // BR: If employee adds part, status is ACTIVE (auto-approved)
    const status = isEmployee ? PartStatus.ACTIVE : PartStatus.PENDING_REVIEW;
    
    // Find customer name for metadata
    let customerName = undefined;
    if (isEmployee) {
      customerName = customers.value.find(c => c.id === form.value.customerId)?.name;
    }

    await partService.createPart({
      ...form.value,
      status,
      customerName
    });
    ElMessage.success(t('part_create.success'));
    router.push({ name: 'parts' });
  } catch (error) {
    console.error('Failed to create part:', error);
  }
};
</script>

<template>
  <div class="page-wrapper">
    <div class="page-container">
      <!-- Breadcrumb Navigation -->
      <nav class="breadcrumb">
        <a href="#" @click.prevent="router.back()">{{ $t('common.menu.parts') }}</a>
        <span class="separator">/</span>
        <span class="current">{{ $t('part_create.title') }}</span>
      </nav>

      <header class="page-header">
        <h1>{{ $t('part_create.title') }}</h1>
      </header>

      <div class="form-layout">
        <Card class="form-card">
          <form @submit.prevent="handleSubmit">
            <!-- Customer Selection (Employee Only) - Mandatory -->
            <div v-if="isEmployee" class="form-group">
              <label>{{ $t('employee.customer_select') }} <span class="required-asterisk">*</span></label>
              <el-select 
                v-model="form.customerId" 
                :placeholder="$t('employee.customer_select')"
                class="form-select-el"
                :class="{ 'is-invalid': errors.customerId }"
                data-test="customer-select"
                filterable
              >
                <el-option
                  v-for="c in customers"
                  :key="c.id"
                  :label="c.name"
                  :value="c.id"
                />
              </el-select>
              <span v-if="errors.customerId" class="error-text">{{ $t(errors.customerId) }}</span>
            </div>

            <div class="form-group">
              <label>{{ $t('customer.part_no') }} <span class="required-asterisk">*</span></label>
              <input 
                v-model="form.partNo" 
                type="text" 
                :placeholder="$t('part_create.part_no_placeholder')"
                class="form-input"
                :class="{ 'is-invalid': errors.partNo }"
                data-test="part-no-input"
              />
              <span v-if="errors.partNo" class="error-text">{{ $t(errors.partNo) }}</span>
            </div>

            <div class="form-group">
              <label>{{ $t('common.supplier') }} <span class="required-asterisk">*</span></label>
              <el-select 
                v-model="form.supplier" 
                :placeholder="$t('part_list.filter_supplier')"
                class="form-select-el"
                :class="{ 'is-invalid': errors.supplier }"
                data-test="supplier-select"
                filterable
              >
                <el-option
                  v-for="s in suppliers"
                  :key="s"
                  :label="s"
                  :value="s"
                />
              </el-select>
              <span v-if="errors.supplier" class="error-text">{{ $t(errors.supplier) }}</span>
            </div>

            <div class="form-group">
              <label>{{ $t('part_create.suggested_hts') }} <span class="required-asterisk">*</span></label>
              <input 
                v-model="form.htsCode" 
                type="text" 
                :placeholder="$t('part_create.hts_code_placeholder')"
                class="form-input"
                :class="{ 'is-invalid': errors.htsCode }"
                data-test="hts-code-input"
              />
              <span v-if="errors.htsCode" class="error-text">{{ $t(errors.htsCode) }}</span>
              <small class="hint-text">{{ $t('part_create.hts_code_placeholder') }}</small>
            </div>

            <div class="form-group">
              <label>{{ $t('part_create.description') }} <span class="required-asterisk">*</span></label>
              <textarea 
                v-model="form.description" 
                :placeholder="$t('part_create.description_placeholder')"
                class="form-textarea"
                :class="{ 'is-invalid': errors.description }"
                data-test="description-input"
              ></textarea>
              <span v-if="errors.description" class="error-text">{{ $t(errors.description) }}</span>
            </div>

            <div class="form-actions">
              <Button type="button" mode="btn-outline" @click="router.back()">
                {{ $t('common.cancel') }}
              </Button>
              <Button type="submit">
                {{ $t('common.save') }}
              </Button>
            </div>
          </form>
        </Card>
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
  padding: 2.5rem 3rem;
  max-width: 1200px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

/* Breadcrumb */
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 2rem;
  font-size: 0.9rem;
}

.breadcrumb a {
  color: var(--primary-color);
  text-decoration: none;
}

.breadcrumb .separator {
  color: #adb5bd;
}

.breadcrumb .current {
  color: #6c757d;
}

.page-header {
  margin-bottom: 2.5rem;
}

h1 {
  font-size: 2rem;
  color: var(--sidebar-color);
  margin: 0;
}

.form-layout {
  display: grid;
  grid-template-columns: 1fr;
  max-width: 800px;
  margin: 0 auto;
}

.form-card {
  padding: 2rem;
  border-radius: 12px;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  font-size: 0.9rem;
  color: #525f7f;
  margin-bottom: 0.5rem;
}

.required-asterisk {
  color: #F56C6C;
  font-weight: bold;
  margin-left: 2px;
}

.form-input, .form-textarea, .form-select-el {
  width: 100%;
  font-family: inherit;
  outline: none;
  transition: border-color 0.2s;
}

.form-select-el {
  border: none !important;
  padding: 0 !important;
}

.form-input, .form-textarea {
  padding: 0.8rem;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  background-color: white;
}

.form-select-el :deep(.el-input__wrapper) {
  padding: 8px 12px;
  border-radius: 8px;
  box-shadow: 0 0 0 1px #dee2e6 inset !important;
}

.form-textarea {
  height: 150px;
  resize: vertical;
}

.is-invalid {
  border-color: #F56C6C;
}

.error-text {
  display: block;
  color: #F56C6C;
  font-size: 0.8rem;
  margin-top: 0.3rem;
}

.hint-text {
  display: block;
  color: #adb5bd;
  font-size: 0.75rem;
  margin-top: 0.2rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #f0f2f5;
}
</style>
