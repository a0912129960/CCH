<script setup lang="ts">
import { authService, UserRole } from '@src/services/auth/auth';
import Card from '@src/components/common/Card.vue';
import Button from '@src/components/common/Button.vue';
import { partService, type CreatePartRequest } from '@src/services/part/part';
import { commonService, type CountryOption } from '@src/services/common/common';
import { ElMessage } from 'element-plus';

/**
 * Part No Creation View (新增零件編號頁面)
 * BR-08: HTS Format Validation | BR-21: Description Quality Scoring
 * Updated: Mandatory Customer ID for Employees and Auto-Activation logic.
 * Update by Gemini AI on 2026-04-18: Global import cleanup and path alias refactor.
 */

const router = useRouter();
const { t } = useI18n();

const { role, customerId: userCustomerId } = authService.state;
const isEmployee = role && role !== UserRole.CUSTOMER;

const countries = ref<CountryOption[]>([]);

const form = ref({
  partNo: '',
  countryOfOrigin: '' as string | number | '',
  division: '',
  supplier: '',
  partDescription: '',
  usHtsCode: '',
  generalDutyRate: '',
  htsCode301: '',
  rate301: '',
  htsCodeIeepa: '',
  rateIeepa: '',
  htsCode232Aluminum: '',
  rate232Aluminum: '',
  htsCodeReciprocalTariff: '',
  rateReciprocalTariff: '',
  remark: '',
  customerId: isEmployee ? '' : userCustomerId || ''
});

const customers = ref<{ id: string; name: string }[]>([]);

onMounted(async () => {
  // Load customers (mock) and countries (real API) independently
  // so a failure in one does not block the other
  try {
    customers.value = await partService.getCustomers();
  } catch (error) {
    console.error('Failed to load customers:', error);
  }

  try {
    const data = await commonService.getCountries();
    countries.value = data;
    if (!data.length) {
      ElMessage.warning(t('part_create.countries_load_failed'));
    }
  } catch (error) {
    console.error('Failed to load countries:', error);
    ElMessage.error(t('part_create.countries_load_failed'));
  }
});

const sanitizeDecimal = (val: string): string =>
  val.replace(/[^\d.]/g, '').replace(/(\..*)\./g, '$1');

watch(() => form.value.usHtsCode, (newVal) => {
  if (!newVal) return;
  const sanitized = sanitizeDecimal(newVal);
  if (newVal !== sanitized) form.value.usHtsCode = sanitized;
});

watch(() => form.value.generalDutyRate, (newVal) => {
  if (!newVal) return;
  const digitsOnly = newVal.replace(/\D/g, '');
  if (newVal !== digitsOnly) form.value.generalDutyRate = digitsOnly;
});

watch(() => form.value.htsCode301, (newVal) => {
  if (!newVal) return;
  const sanitized = sanitizeDecimal(newVal);
  if (newVal !== sanitized) form.value.htsCode301 = sanitized;
});

watch(() => form.value.rate301, (newVal) => {
  if (!newVal) return;
  const digitsOnly = newVal.replace(/\D/g, '');
  if (newVal !== digitsOnly) form.value.rate301 = digitsOnly;
});

watch(() => form.value.htsCodeIeepa, (newVal) => {
  if (!newVal) return;
  const sanitized = sanitizeDecimal(newVal);
  if (newVal !== sanitized) form.value.htsCodeIeepa = sanitized;
});

watch(() => form.value.rateIeepa, (newVal) => {
  if (!newVal) return;
  const digitsOnly = newVal.replace(/\D/g, '');
  if (newVal !== digitsOnly) form.value.rateIeepa = digitsOnly;
});

watch(() => form.value.htsCode232Aluminum, (newVal) => {
  if (!newVal) return;
  const sanitized = sanitizeDecimal(newVal);
  if (newVal !== sanitized) form.value.htsCode232Aluminum = sanitized;
});

watch(() => form.value.rate232Aluminum, (newVal) => {
  if (!newVal) return;
  const digitsOnly = newVal.replace(/\D/g, '');
  if (newVal !== digitsOnly) form.value.rate232Aluminum = digitsOnly;
});

watch(() => form.value.htsCodeReciprocalTariff, (newVal) => {
  if (!newVal) return;
  const sanitized = sanitizeDecimal(newVal);
  if (newVal !== sanitized) form.value.htsCodeReciprocalTariff = sanitized;
});

watch(() => form.value.rateReciprocalTariff, (newVal) => {
  if (!newVal) return;
  const digitsOnly = newVal.replace(/\D/g, '');
  if (newVal !== digitsOnly) form.value.rateReciprocalTariff = digitsOnly;
});

const errors = ref({
  partNo: '',
  countryOfOrigin: '',
  division: '',
  supplier: '',
  partDescription: '',
  usHtsCode: '',
  customerId: ''
});

// Save：只檢查 Part No & Country of Origin
const validateSave = () => {
  let valid = true;
  errors.value = { partNo: '', countryOfOrigin: '', division: '', supplier: '', partDescription: '', usHtsCode: '', customerId: '' };

  if (!form.value.partNo) {
    errors.value.partNo = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.countryOfOrigin) {
    errors.value.countryOfOrigin = 'part_create.validation.required';
    valid = false;
  }
  if (isEmployee && !form.value.customerId) {
    errors.value.customerId = 'part_create.validation.required';
    valid = false;
  }

  return valid;
};

// Save & Submit：檢查所有必填欄位
const validateSaveAndSubmit = () => {
  let valid = true;
  errors.value = { partNo: '', countryOfOrigin: '', division: '', supplier: '', partDescription: '', usHtsCode: '', customerId: '' };

  if (!form.value.partNo) {
    errors.value.partNo = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.countryOfOrigin) {
    errors.value.countryOfOrigin = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.division) {
    errors.value.division = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.supplier) {
    errors.value.supplier = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.partDescription) {
    errors.value.partDescription = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.usHtsCode) {
    errors.value.usHtsCode = 'part_create.validation.required';
    valid = false;
  }
  if (isEmployee && !form.value.customerId) {
    errors.value.customerId = 'part_create.validation.required';
    valid = false;
  }

  return valid;
};

const toNum = (val: string): number | undefined =>
  val !== '' ? Number(val) : undefined;

// Convert the selected country key to the correct type for POST body.
// If the key is already a number (numeric id) → use as-is.
// If it's a numeric string like "158" → parse to number.
// If it's a string code like "USA" → keep as string (backend may accept both).
const toCountryId = (val: string | number | ''): string | number | undefined => {
  if (val === '') return undefined;
  if (typeof val === 'number') return val;
  const n = Number(val);
  return Number.isNaN(n) ? val : n;  // numeric string → number; code string → keep as-is
};

const handleSubmit = async () => {
  if (!validateSave()) return;

  try {
    const body: CreatePartRequest = {
      customerId:  form.value.customerId  || undefined,
      partNo:      form.value.partNo,
      countryId:   toCountryId(form.value.countryOfOrigin),
      division:    form.value.division    || undefined,
      supplier:    form.value.supplier    || undefined,
      partDesc:    form.value.partDescription || undefined,
      htsCode:     form.value.usHtsCode   || undefined,
      rate:        toNum(form.value.generalDutyRate),
      htsCode1:    form.value.htsCode301  || undefined,
      rate1:       toNum(form.value.rate301),
      htsCode2:    form.value.htsCodeIeepa || undefined,
      rate2:       toNum(form.value.rateIeepa),
      htsCode3:    form.value.htsCode232Aluminum || undefined,
      rate3:       toNum(form.value.rate232Aluminum),
      htsCode4:    form.value.htsCodeReciprocalTariff || undefined,
      rate4:       toNum(form.value.rateReciprocalTariff),
      remark:      form.value.remark      || undefined
    };

    const result = await partService.createPartApi(body);

    if (result.success) {
      ElMessage.success(result.message || t('part_create.success'));
      router.push({ name: 'parts' });
    }
  } catch (error) {
    console.error('Failed to create part:', error);
  }
};

const handleSaveAndSubmit = async () => {
  if (!validateSaveAndSubmit()) return;

  try {
    const body: CreatePartRequest = {
      customerId:  form.value.customerId  || undefined,
      partNo:      form.value.partNo,
      countryId:   toCountryId(form.value.countryOfOrigin),
      division:    form.value.division    || undefined,
      supplier:    form.value.supplier    || undefined,
      partDesc:    form.value.partDescription || undefined,
      htsCode:     form.value.usHtsCode   || undefined,
      rate:        toNum(form.value.generalDutyRate),
      htsCode1:    form.value.htsCode301  || undefined,
      rate1:       toNum(form.value.rate301),
      htsCode2:    form.value.htsCodeIeepa || undefined,
      rate2:       toNum(form.value.rateIeepa),
      htsCode3:    form.value.htsCode232Aluminum || undefined,
      rate3:       toNum(form.value.rate232Aluminum),
      htsCode4:    form.value.htsCodeReciprocalTariff || undefined,
      rate4:       toNum(form.value.rateReciprocalTariff),
      remark:      form.value.remark      || undefined
    };

    const result = await partService.submitPartApi(body);

    if (result.success) {
      ElMessage.success(result.message || t('part_create.save_and_submit_success'));
      router.push({ name: 'parts' });
    }
  } catch (error) {
    console.error('Failed to save and submit part:', error);
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

            <!-- ── Part Information Table ── -->
            <div class="info-table">
              <div class="info-table__title">{{ $t('part_create.part_information') }}</div>
              <div class="info-table__header">
                <span>{{ $t('part_create.col_field') }}</span>
                <span>{{ $t('part_create.col_value') }}</span>
              </div>

              <!-- Customer (Employee Only) -->
              <div v-if="isEmployee" class="info-table__row">
                <div class="info-table__field">
                  {{ $t('employee.customer_select') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <el-select
                    v-model="form.customerId"
                    :placeholder="$t('employee.customer_select')"
                    class="form-select-el"
                    :class="{ 'is-invalid': errors.customerId }"
                    data-test="customer-select"
                    filterable
                  >
                    <el-option v-for="c in customers" :key="c.id" :label="c.name" :value="c.id" />
                  </el-select>
                  <span v-if="errors.customerId" class="error-text">{{ $t(errors.customerId) }}</span>
                </div>
              </div>

              <!-- Part No -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('customer.part_no') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.partNo" type="text" :placeholder="$t('part_create.part_no_placeholder')" class="form-input" :class="{ 'is-invalid': errors.partNo }" data-test="part-no-input" />
                  <span v-if="errors.partNo" class="error-text">{{ $t(errors.partNo) }}</span>
                  <span class="immutable-hint">{{ $t('part_create.immutable_hint') }}</span>
                </div>
              </div>

              <!-- Country of Origin -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('part_create.country_of_origin') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <el-select v-model="form.countryOfOrigin" :placeholder="$t('part_create.country_of_origin_placeholder')" class="form-select-el" :class="{ 'is-invalid': errors.countryOfOrigin }" data-test="country-of-origin-select" filterable clearable>
                    <el-option v-for="country in countries" :key="country.key" :label="country.value" :value="country.key" />
                  </el-select>
                  <span v-if="errors.countryOfOrigin" class="error-text">{{ $t(errors.countryOfOrigin) }}</span>
                  <span class="immutable-hint">{{ $t('part_create.immutable_hint') }}</span>
                </div>
              </div>

              <!-- Division -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('part_create.division') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.division" type="text" :placeholder="$t('part_create.division_placeholder')" class="form-input" :class="{ 'is-invalid': errors.division }" data-test="division-input" />
                  <span v-if="errors.division" class="error-text">{{ $t(errors.division) }}</span>
                </div>
              </div>

              <!-- Supplier -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('common.supplier') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.supplier" type="text" :placeholder="$t('part_list.filter_supplier')" class="form-input" :class="{ 'is-invalid': errors.supplier }" data-test="supplier-input" />
                  <span v-if="errors.supplier" class="error-text">{{ $t(errors.supplier) }}</span>
                </div>
              </div>

              <!-- Part Description -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('part_create.part_description') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.partDescription" type="text" :placeholder="$t('part_create.part_description_placeholder')" class="form-input" :class="{ 'is-invalid': errors.partDescription }" data-test="part-description-input" />
                  <span v-if="errors.partDescription" class="error-text">{{ $t(errors.partDescription) }}</span>
                </div>
              </div>

              <!-- US HTS Code -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('part_create.us_hts_code') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.usHtsCode" type="text" :placeholder="$t('part_create.us_hts_code_placeholder')" class="form-input" :class="{ 'is-invalid': errors.usHtsCode }" data-test="us-hts-code-input" />
                  <span v-if="errors.usHtsCode" class="error-text">{{ $t(errors.usHtsCode) }}</span>
                </div>
              </div>

              <!-- General Duty Rate -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.general_duty_rate') }}</div>
                <div class="info-table__value">
                  <input v-model="form.generalDutyRate" type="text" :placeholder="$t('part_create.general_duty_rate_placeholder')" class="form-input" data-test="general-duty-rate-input" />
                </div>
              </div>

              <!-- Section: Additional Duty -->
              <div class="info-table__section-header">
                <span>{{ $t('part_create.additional_duty') }}</span>
              </div>

              <!-- HTS Code (301) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_301') }}</div>
                <div class="info-table__value">
                  <input v-model="form.htsCode301" type="text" :placeholder="$t('part_create.hts_code_301_placeholder')" class="form-input" data-test="hts-code-301-input" />
                </div>
              </div>

              <!-- Rate (301) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_301') }}</div>
                <div class="info-table__value">
                  <input v-model="form.rate301" type="text" :placeholder="$t('part_create.rate_301_placeholder')" class="form-input" data-test="rate-301-input" />
                </div>
              </div>

              <!-- HTS Code (IEEPA) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_ieepa') }}</div>
                <div class="info-table__value">
                  <input v-model="form.htsCodeIeepa" type="text" :placeholder="$t('part_create.hts_code_ieepa_placeholder')" class="form-input" data-test="hts-code-ieepa-input" />
                </div>
              </div>

              <!-- Rate (IEEPA) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_ieepa') }}</div>
                <div class="info-table__value">
                  <input v-model="form.rateIeepa" type="text" :placeholder="$t('part_create.rate_ieepa_placeholder')" class="form-input" data-test="rate-ieepa-input" />
                </div>
              </div>

              <!-- HTS Code (232 Aluminum) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_232_aluminum') }}</div>
                <div class="info-table__value">
                  <input v-model="form.htsCode232Aluminum" type="text" :placeholder="$t('part_create.hts_code_232_aluminum_placeholder')" class="form-input" data-test="hts-code-232-aluminum-input" />
                </div>
              </div>

              <!-- Rate (232 Aluminum) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_232_aluminum') }}</div>
                <div class="info-table__value">
                  <input v-model="form.rate232Aluminum" type="text" :placeholder="$t('part_create.rate_232_aluminum_placeholder')" class="form-input" data-test="rate-232-aluminum-input" />
                </div>
              </div>

              <!-- HTS Code (Reciprocal Tariff) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_reciprocal_tariff') }}</div>
                <div class="info-table__value">
                  <input v-model="form.htsCodeReciprocalTariff" type="text" :placeholder="$t('part_create.hts_code_reciprocal_tariff_placeholder')" class="form-input" data-test="hts-code-reciprocal-tariff-input" />
                </div>
              </div>

              <!-- Rate (Reciprocal Tariff) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_reciprocal_tariff') }}</div>
                <div class="info-table__value">
                  <input v-model="form.rateReciprocalTariff" type="text" :placeholder="$t('part_create.rate_reciprocal_tariff_placeholder')" class="form-input" data-test="rate-reciprocal-tariff-input" />
                </div>
              </div>

              <!-- Remark -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.remark') }}</div>
                <div class="info-table__value">
                  <textarea v-model="form.remark" :placeholder="$t('part_create.remark_placeholder')" class="form-textarea" data-test="remark-input"></textarea>
                </div>
              </div>

            </div>
            <!-- ── End Part Information Table ── -->

            <div class="form-actions">
              <Button type="submit">
                {{ $t('common.save') }}
              </Button>
              <Button type="button" @click="handleSaveAndSubmit" data-test="save-submit-btn">
                {{ $t('part_create.save_and_submit') }}
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
  margin: 0 auto;
}

.form-card {
  padding: 0;
  border-radius: 12px;
  overflow: hidden;
}

/* ── Info Table ── */
.info-table {
  width: 100%;
  border: 1px solid #e4e7ed;
  border-radius: 10px;
  overflow: hidden;
}

.info-table__title {
  padding: 1rem 1.5rem;
  font-size: 1rem;
  font-weight: bold;
  color: #525f7f;
  background-color: #fff;
  border-bottom: 1px solid #e4e7ed;
}

.info-table__header {
  display: grid;
  grid-template-columns: 280px 1fr;
  background-color: #f5f7fa;
  border-bottom: 1px solid #e4e7ed;
  padding: 0.6rem 1.5rem;
  font-size: 0.85rem;
  font-weight: 600;
  color: #8492a6;
}

.info-table__row {
  display: grid;
  grid-template-columns: 280px 1fr;
  border-bottom: 1px solid #ebeef5;
  align-items: center;
  min-height: 56px;
}

.info-table__row:last-child {
  border-bottom: none;
}

.info-table__field {
  padding: 0.75rem 1.5rem;
  font-size: 0.9rem;
  font-weight: 600;
  color: #525f7f;
  align-self: stretch;
  display: flex;
  align-items: center;
}

.info-table__value {
  padding: 0.6rem 1.5rem;
}

.info-table__section-header {
  grid-column: 1 / -1;
  background-color: #f0f2f5;
  border-top: 1px solid #e4e7ed;
  border-bottom: 1px solid #e4e7ed;
  padding: 0.65rem 1.5rem;
  font-size: 0.9rem;
  font-weight: bold;
  color: #525f7f;
  text-align: center;
}

/* ── Inputs ── */
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
  padding: 0.6rem 0.8rem;
  border: 1px solid #dee2e6;
  border-radius: 6px;
  background-color: white;
  font-size: 0.9rem;
}

.form-select-el :deep(.el-input__wrapper) {
  padding: 6px 12px;
  border-radius: 6px;
  box-shadow: 0 0 0 1px #dee2e6 inset !important;
}

.form-textarea {
  height: 120px;
  resize: vertical;
}

.is-invalid {
  border-color: #F56C6C;
}

.error-text {
  display: block;
  color: #F56C6C;
  font-size: 0.8rem;
  margin-top: 0.25rem;
}

.immutable-hint {
  display: block;
  color: #E6A23C;
  font-size: 0.78rem;
  margin-top: 0.25rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1.5rem;
}
</style>
