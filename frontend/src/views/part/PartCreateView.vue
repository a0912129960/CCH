<script setup lang="ts">
import { authService } from '@src/services/auth/auth';
import Card from '@src/components/common/Card.vue';
import Button from '@src/components/common/Button.vue';
import { partService, getHtsRecommendation, type CreatePartRequest, type SubmitPartRequest } from '@src/services/part/part';
import { commonService, type CountryOption, type ProjectOption } from '@src/services/common/common';
import { ElMessage } from 'element-plus';
import { useTabStore } from '@src/stores/tabs';

/**
 * Part No Creation View (新增零件編號頁面)
 * BR-08: HTS Format Validation | BR-21: Description Quality Scoring
 * Update by Gemini AI on 2026-04-23: Enable Project Selection for all users (including Customers).
 */

const router = useRouter();
const route = useRoute();
const { t } = useI18n();
const tabStore = useTabStore();

const { projectId: userProjectId } = authService.state;

// Prefer projectId from route query (passed from PartListView's project filter),
// fall back to the logged-in user's own projectId (客戶使用者自己的 ID).
const resolvedprojectId = (route.query.projectId as string) || userProjectId || '';

// Resolved project display name (for read-only display)
const resolvedProjectName = computed(() => {
  if (!form.value.projectId) return '';
  const found = projects.value.find(p => p.key === form.value.projectId);
  return found ? found.value : form.value.projectId;
});

const countries = ref<CountryOption[]>([]);
const projects = ref<ProjectOption[]>([]);

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
  projectId: resolvedprojectId
});

onMounted(async () => {
  try {
    const [countriesData, projectsData] = await Promise.all([
      commonService.getCountries(),
      commonService.getProjects()
    ]);
    
    countries.value = countriesData;
    projects.value = projectsData;

    if (!countriesData.length) {
      ElMessage.warning(t('part_create.countries_load_failed'));
    }
  } catch (error) {
    console.error('Failed to load initial data:', error);
    ElMessage.error(t('part_create.initial_load_failed'));
  }
});

// Rate field input — only allow digits and a single decimal point (只允許數字和一個小數點)
type RateField = 'generalDutyRate' | 'rate301' | 'rateIeepa' | 'rate232Aluminum' | 'rateReciprocalTariff';

const handleRateInput = (field: RateField, event: Event) => {
  const input = event.target as HTMLInputElement;
  // Remove anything that is not a digit or a dot
  let cleaned = input.value.replace(/[^\d.]/g, '');
  // Allow only the first decimal point
  const dotIndex = cleaned.indexOf('.');
  if (dotIndex !== -1) {
    cleaned = cleaned.slice(0, dotIndex + 1) + cleaned.slice(dotIndex + 1).replace(/\./g, '');
  }
  (form.value as any)[field] = cleaned;
  input.value = cleaned; // sync DOM immediately
};

// HTS Code auto-format — same logic as PartDetailView (與 PartDetailView 共用相同邏輯)
// Strip non-digits, insert dots at XXXX.XX.XXXX positions on every keystroke.
const HTS_PATTERN = /^\d{4}\.\d{2}\.\d{4}$/;
const htsError     = ref('');
const htsCode1Error = ref('');
const htsCode2Error = ref('');
const htsCode3Error = ref('');
const htsCode4Error = ref('');

const validateHts = (val: string | null | undefined, errRef: any): boolean => {
  if (!val) { 
    if (errRef && typeof errRef === 'object' && 'value' in errRef) errRef.value = '';
    return true; 
  }
  if (!HTS_PATTERN.test(val)) {
    const msg = 'Format must be XXXX.XX.XXXX';
    if (errRef && typeof errRef === 'object' && 'value' in errRef) errRef.value = msg;
    return false;
  }
  if (errRef && typeof errRef === 'object' && 'value' in errRef) errRef.value = '';
  return true;
};

type HtsField = 'usHtsCode' | 'htsCode301' | 'htsCodeIeepa' | 'htsCode232Aluminum' | 'htsCodeReciprocalTariff';

const formatHtsCode = (raw: string): string => {
  const digits = raw.replace(/\D/g, '').slice(0, 10);
  if (digits.length <= 4) return digits;
  if (digits.length <= 6) return `${digits.slice(0, 4)}.${digits.slice(4)}`;
  return `${digits.slice(0, 4)}.${digits.slice(4, 6)}.${digits.slice(6)}`;
};

// Tracks USITC existence check result for US HTS Code only.
// null = not checked, true = found, false = not found.
const htsExists = ref<boolean | null>(null);
const htsLookupLoading = ref(false);

const handleHtsInput = (field: HtsField, errRef: any, event: Event) => {
  const input = event.target as HTMLInputElement;
  const formatted = formatHtsCode(input.value);
  (form.value as any)[field] = formatted;
  input.value = formatted;
  validateHts(formatted, errRef);
  if (field === 'usHtsCode') htsExists.value = null;
};

const handleHtsCodeBlur = async () => {
  validateHts(form.value.usHtsCode, htsError);
  if (htsError.value || !form.value.usHtsCode) return;

  htsLookupLoading.value = true;
  try {
    const result = await getHtsRecommendation(form.value.usHtsCode);
    if (!result) return;

    if (!result.fallback_used && result.message === 'No recommendation data') {
      htsExists.value = false;
      htsError.value = 'HTS Code not found on hts.usitc.gov';
      ElMessage.warning('HTS Code not found on hts.usitc.gov');
      return;
    }

    htsExists.value = true;

    if (result.fallback_used && result.data?.general) {
      const raw = result.data.general.replace('%', '').trim();
      const rate = raw.toLowerCase() === 'free' ? 0 : parseFloat(raw);
      if (!isNaN(rate)) {
        form.value.generalDutyRate = String(rate);
        ElMessage.info('General Duty Rate auto-filled from HTS recommendation.');
      }
    }
  } finally {
    htsLookupLoading.value = false;
  }
};

const errors = ref({
  projectId: '',
  partNo: '',
  countryOfOrigin: '',
  division: '',
  supplier: '',
  partDescription: '',
  usHtsCode: ''
});

// Save validation
const validateSave = () => {
  let valid = true;
  errors.value = { projectId: '', partNo: '', countryOfOrigin: '', division: '', supplier: '', partDescription: '', usHtsCode: '' };

  if (!form.value.projectId) {
    errors.value.projectId = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.partNo) {
    errors.value.partNo = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.countryOfOrigin) {
    errors.value.countryOfOrigin = 'part_create.validation.required';
    valid = false;
  }

  return valid;
};

// Save & Submit validation
const validateSaveAndSubmit = () => {
  let valid = true;
  errors.value = { projectId: '', partNo: '', countryOfOrigin: '', division: '', supplier: '', partDescription: '', usHtsCode: '' };

  if (!form.value.projectId) {
    errors.value.projectId = 'part_create.validation.required';
    valid = false;
  }
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

  return valid;
};

const toNum = (val: string): number | undefined =>
  val !== '' ? Number(val) : undefined;

const toCountryId = (val: string | number | ''): string | number | undefined => {
  if (val === '') return undefined;
  if (typeof val === 'number') return val;
  const n = Number(val);
  return Number.isNaN(n) ? val : n;
};

/**
 * Shared duplicate check — called before both Save and Save & Submit.
 * Returns true if a duplicate exists (caller should abort); false if safe to proceed.
 * (共用查重邏輯，在 Save 與 Save & Submit 前調用。重複時返回 true，可繼續時返回 false。)
 */
const checkDuplicateBeforeSave = async (): Promise<boolean> => {
  const countryId = toCountryId(form.value.countryOfOrigin);
  const projectId = form.value.projectId;

  // Need both fields to perform the check (兩個欄位都需要才能查重)
  if (!form.value.partNo || !countryId || !projectId) return false;

  const isDuplicate = await partService.checkDuplicate(projectId, form.value.partNo, countryId);
  if (isDuplicate) {
    await ElMessageBox.alert(
      t('part_create.duplicate_error') ||
        'A record with the same Part No and Country of Origin already exists. Part No and Country of Origin must be unique.\n（相同的零件編號與原產地組合已存在，Part No 與 Country of Origin 必須唯一，不可重複。）',
      t('part_create.duplicate_error_title') || 'Duplicate Entry',
      { type: 'error', confirmButtonText: t('common.ok') || 'OK' }
    );
    return true;
  }
  return false;
};

// [Save] → PartCreateRequest: only partNo + countryId required
const handleSubmit = async () => {
  if (!validateSave()) return;
  if (await checkDuplicateBeforeSave()) return;

  try {
    const body: CreatePartRequest = {
      projectId:   form.value.projectId  || undefined,
      partNo:      form.value.partNo,
      countryId:   toCountryId(form.value.countryOfOrigin)!,
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
      remark:      form.value.remark      || undefined,
      isHTSExists: htsExists.value
    };

    const result = await partService.createPartApi(body);

    if (result.success) {
      ElMessage.success(result.message || t('part_create.success'));
      tabStore.refreshTab('/parts');
      router.push({ name: 'parts', query: form.value.projectId ? { projectId: form.value.projectId } : {} });
    }
  } catch (error) {
    console.error('Failed to create part:', error);
  }
};

const handleSaveAndSubmit = async () => {
  if (!validateSaveAndSubmit()) return;
  if (await checkDuplicateBeforeSave()) return;

  try {
    const body: SubmitPartRequest = {
      projectId:   form.value.projectId  || undefined,
      partNo:      form.value.partNo,
      countryId:   toCountryId(form.value.countryOfOrigin) as number,
      division:    form.value.division,
      supplier:    form.value.supplier,
      partDesc:    form.value.partDescription,
      htsCode:     form.value.usHtsCode,
      rate:        toNum(form.value.generalDutyRate),
      htsCode1:    form.value.htsCode301  || undefined,
      rate1:       toNum(form.value.rate301),
      htsCode2:    form.value.htsCodeIeepa || undefined,
      rate2:       toNum(form.value.rateIeepa),
      htsCode3:    form.value.htsCode232Aluminum || undefined,
      rate3:       toNum(form.value.rate232Aluminum),
      htsCode4:    form.value.htsCodeReciprocalTariff || undefined,
      rate4:       toNum(form.value.rateReciprocalTariff),
      remark:      form.value.remark      || undefined,
      isHTSExists: htsExists.value
    };

    const result = await partService.submitPartApi(body);

    if (result.success) {
      ElMessage.success(result.message || t('part_create.save_and_submit_success'));
      tabStore.refreshTab('/parts');
      router.push({ name: 'parts', query: form.value.projectId ? { projectId: form.value.projectId } : {} });
    }
  } catch (error) {
    console.error('Failed to save and submit part:', error);
  }
};
</script>

<template>
  <div class="page-wrapper">
    <div class="page-container">
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
            <div class="info-table">
              <div class="info-table__title">{{ $t('part_create.part_information') }}</div>
              <div class="info-table__header">
                <span>{{ $t('part_create.col_field') }}</span>
                <span>{{ $t('part_create.col_value') }}</span>
              </div>

              <!-- Project (read-only, sourced from Parts List selection) -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('common.project') }} <span class="required-asterisk">*</span>
                </div>
                <div class="info-table__value">
                  <span class="readonly-value" data-test="project-readonly">{{ resolvedProjectName || form.projectId || '-' }}</span>
                  <span v-if="errors.projectId" class="error-text">{{ $t(errors.projectId) }}</span>
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
                  {{ $t('part_create.division') }} <span class="required-asterisk submit-only-asterisk" :title="$t('part_create.required_for_submit')">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.division" type="text" :placeholder="$t('part_create.division_placeholder')" class="form-input" :class="{ 'is-invalid': errors.division }" data-test="division-input" />
                  <span v-if="errors.division" class="error-text">{{ $t(errors.division) }}</span>
                </div>
              </div>

              <!-- Supplier -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('common.supplier') }} <span class="required-asterisk submit-only-asterisk" :title="$t('part_create.required_for_submit')">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.supplier" type="text" :placeholder="$t('part_list.filter_supplier')" class="form-input" :class="{ 'is-invalid': errors.supplier }" data-test="supplier-input" />
                  <span v-if="errors.supplier" class="error-text">{{ $t(errors.supplier) }}</span>
                </div>
              </div>

              <!-- Part Description -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('part_create.part_description') }} <span class="required-asterisk submit-only-asterisk" :title="$t('part_create.required_for_submit')">*</span>
                </div>
                <div class="info-table__value">
                  <input v-model="form.partDescription" type="text" :placeholder="$t('part_create.part_description_placeholder')" class="form-input" :class="{ 'is-invalid': errors.partDescription }" data-test="part-description-input" />
                  <span v-if="errors.partDescription" class="error-text">{{ $t(errors.partDescription) }}</span>
                </div>
              </div>

              <!-- US HTS Code -->
              <div class="info-table__row">
                <div class="info-table__field">
                  {{ $t('part_create.us_hts_code') }} <span class="required-asterisk submit-only-asterisk" :title="$t('part_create.required_for_submit')">*</span>
                </div>
                <div class="info-table__value">
                  <input :value="form.usHtsCode" type="text" :placeholder="$t('part_create.us_hts_code_placeholder')" class="form-input" :class="{ 'is-invalid': errors.usHtsCode || htsError }" data-test="us-hts-code-input" inputmode="numeric" :disabled="htsLookupLoading" @input="handleHtsInput('usHtsCode', htsError, $event)" @blur="handleHtsCodeBlur" />
                  <span v-if="errors.usHtsCode" class="error-text">{{ $t(errors.usHtsCode) }}</span>
                  <div v-if="htsError" class="error-text">{{ htsError }}</div>
                  <div v-if="htsLookupLoading" class="field-hint">Looking up rate...</div>
                  <template v-else-if="form.usHtsCode">
                    <span v-if="htsExists === true"  class="hts-badge hts-found">✓ Verified on USITC</span>
                    <span v-else-if="htsExists === false" class="hts-badge hts-not-found">✗ Not found on USITC</span>
                    <span v-else class="hts-badge hts-unknown">— Not yet verified</span>
                  </template>
                </div>
              </div>

              <!-- General Duty Rate -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.general_duty_rate') }}</div>
                <div class="info-table__value">
                  <input :value="form.generalDutyRate" type="text" inputmode="decimal" :placeholder="$t('part_create.general_duty_rate_placeholder')" class="form-input" data-test="general-duty-rate-input" @input="handleRateInput('generalDutyRate', $event)" />
                </div>
              </div>

              <div class="info-table__section-header">
                <span>{{ $t('part_create.additional_duty') }}</span>
              </div>

              <!-- HTS Code (301) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_301') }}</div>
                <div class="info-table__value">
                  <input :value="form.htsCode301" type="text" :placeholder="$t('part_create.hts_code_301_placeholder')" class="form-input" :class="{ 'input-error': htsCode1Error }" data-test="hts-code-301-input" inputmode="numeric" @input="handleHtsInput('htsCode301', htsCode1Error, $event)" @blur="validateHts(form.htsCode301, htsCode1Error)" />
                  <div v-if="htsCode1Error" class="error-text">{{ htsCode1Error }}</div>
                </div>
              </div>

              <!-- Rate (301) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_301') }}</div>
                <div class="info-table__value">
                  <input :value="form.rate301" type="text" inputmode="decimal" :placeholder="$t('part_create.rate_301_placeholder')" class="form-input" data-test="rate-301-input" @input="handleRateInput('rate301', $event)" />
                </div>
              </div>

              <!-- HTS Code (IEEPA) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_ieepa') }}</div>
                <div class="info-table__value">
                  <input :value="form.htsCodeIeepa" type="text" :placeholder="$t('part_create.hts_code_ieepa_placeholder')" class="form-input" :class="{ 'input-error': htsCode2Error }" data-test="hts-code-ieepa-input" inputmode="numeric" @input="handleHtsInput('htsCodeIeepa', htsCode2Error, $event)" @blur="validateHts(form.htsCodeIeepa, htsCode2Error)" />
                  <div v-if="htsCode2Error" class="error-text">{{ htsCode2Error }}</div>
                </div>
              </div>

              <!-- Rate (IEEPA) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_ieepa') }}</div>
                <div class="info-table__value">
                  <input :value="form.rateIeepa" type="text" inputmode="decimal" :placeholder="$t('part_create.rate_ieepa_placeholder')" class="form-input" data-test="rate-ieepa-input" @input="handleRateInput('rateIeepa', $event)" />
                </div>
              </div>

              <!-- HTS Code (232 Aluminum) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_232_aluminum') }}</div>
                <div class="info-table__value">
                  <input :value="form.htsCode232Aluminum" type="text" :placeholder="$t('part_create.hts_code_232_aluminum_placeholder')" class="form-input" :class="{ 'input-error': htsCode3Error }" data-test="hts-code-232-aluminum-input" inputmode="numeric" @input="handleHtsInput('htsCode232Aluminum', htsCode3Error, $event)" @blur="validateHts(form.htsCode232Aluminum, htsCode3Error)" />
                  <div v-if="htsCode3Error" class="error-text">{{ htsCode3Error }}</div>
                </div>
              </div>

              <!-- Rate (232 Aluminum) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_232_aluminum') }}</div>
                <div class="info-table__value">
                  <input :value="form.rate232Aluminum" type="text" inputmode="decimal" :placeholder="$t('part_create.rate_232_aluminum_placeholder')" class="form-input" data-test="rate-232-aluminum-input" @input="handleRateInput('rate232Aluminum', $event)" />
                </div>
              </div>

              <!-- HTS Code (Reciprocal Tariff) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.hts_code_reciprocal_tariff') }}</div>
                <div class="info-table__value">
                  <input :value="form.htsCodeReciprocalTariff" type="text" :placeholder="$t('part_create.hts_code_reciprocal_tariff_placeholder')" class="form-input" :class="{ 'input-error': htsCode4Error }" data-test="hts-code-reciprocal-tariff-input" inputmode="numeric" @input="handleHtsInput('htsCodeReciprocalTariff', htsCode4Error, $event)" @blur="validateHts(form.htsCodeReciprocalTariff, htsCode4Error)" />
                  <div v-if="htsCode4Error" class="error-text">{{ htsCode4Error }}</div>
                </div>
              </div>

              <!-- Rate (Reciprocal Tariff) -->
              <div class="info-table__row">
                <div class="info-table__field">{{ $t('part_create.rate_reciprocal_tariff') }}</div>
                <div class="info-table__value">
                  <input :value="form.rateReciprocalTariff" type="text" inputmode="decimal" :placeholder="$t('part_create.rate_reciprocal_tariff_placeholder')" class="form-input" data-test="rate-reciprocal-tariff-input" @input="handleRateInput('rateReciprocalTariff', $event)" />
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

            <div class="form-actions">
              <Button type="submit">{{ $t('common.save') }}</Button>
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
.page-wrapper { background-color: var(--dashboard-bg); min-height: 100vh; }
.page-container { padding: 2.5rem 3rem; max-width: 1200px; margin: 0 auto; font-family: "MyDimerco-WorkSansBold", sans-serif; }
.breadcrumb { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 2rem; font-size: 0.9rem; }
.breadcrumb a { color: var(--primary-color); text-decoration: none; }
.breadcrumb .separator { color: #adb5bd; }
.breadcrumb .current { color: #6c757d; }
.page-header { margin-bottom: 2.5rem; }
h1 { font-size: 2rem; color: var(--sidebar-color); margin: 0; }
.form-card { padding: 0; border-radius: 12px; overflow: hidden; }
.info-table { width: 100%; border: 1px solid #e4e7ed; border-radius: 10px; overflow: hidden; }
.info-table__title { padding: 1rem 1.5rem; font-size: 1rem; font-weight: bold; color: #525f7f; background-color: #fff; border-bottom: 1px solid #e4e7ed; }
.info-table__header { display: grid; grid-template-columns: 280px 1fr; background-color: #f5f7fa; border-bottom: 1px solid #e4e7ed; padding: 0.6rem 1.5rem; font-size: 0.85rem; font-weight: 600; color: #8492a6; }
.info-table__row { display: grid; grid-template-columns: 280px 1fr; border-bottom: 1px solid #ebeef5; align-items: center; min-height: 56px; }
.info-table__row:last-child { border-bottom: none; }
.info-table__field { padding: 0.75rem 1.5rem; font-size: 0.9rem; font-weight: 600; color: #525f7f; align-self: stretch; display: flex; align-items: center; }
.info-table__value { padding: 0.6rem 1.5rem; }
.info-table__section-header { grid-column: 1 / -1; background-color: #f0f2f5; border-top: 1px solid #e4e7ed; border-bottom: 1px solid #e4e7ed; padding: 0.65rem 1.5rem; font-size: 0.9rem; font-weight: bold; color: #525f7f; text-align: center; }
.required-asterisk { color: #F56C6C; font-weight: bold; margin-left: 2px; }
.submit-only-asterisk { color: #E6A23C; cursor: help; }
.form-input, .form-textarea, .form-select-el { width: 100%; font-family: inherit; outline: none; transition: border-color 0.2s; }
.form-select-el { border: none !important; padding: 0 !important; }
.form-input, .form-textarea { padding: 0.6rem 0.8rem; border: 1px solid #dee2e6; border-radius: 6px; background-color: white; font-size: 0.9rem; }
.form-select-el :deep(.el-input__wrapper) { padding: 6px 12px; border-radius: 6px; box-shadow: 0 0 0 1px #dee2e6 inset !important; }
.form-textarea { height: 120px; resize: vertical; }
.is-invalid { border-color: #F56C6C; }
.error-text { display: block; color: #F56C6C; font-size: 0.8rem; margin-top: 0.25rem; }
.immutable-hint { display: block; color: #E6A23C; font-size: 0.78rem; margin-top: 0.25rem; }
.form-actions { display: flex; justify-content: flex-end; gap: 1rem; margin-top: 1.5rem; }
.readonly-value { display: inline-block; padding: 0.6rem 0.8rem; font-size: 0.9rem; color: #525f7f; font-weight: 600; }
</style>
