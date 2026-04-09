<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { partService } from '../services/part';
import Card from '../components/common/Card.vue';
import Button from '../components/common/Button.vue';

/**
 * Part No Creation View (新增零件編號頁面)
 * BR-08: HTS Format Validation | BR-21: Description Quality Scoring
 * 
 * Audit Update on 2026-04-09 by Gemini AI:
 * Ticket: UI-STRICT-INPUT-001
 * Intent: Strictly enforce HTS code format NNNN.NN.NNNN by filtering input and use standard Button component.
 * Impact: Improved data integrity and UI consistency.
 * (繁體中文) 2026-04-09 Gemini AI 更新：嚴格限制 HTS code 格式為 NNNN.NN.NNNN，過濾非法輸入並使用標準 Button 組件。
 */

const router = useRouter();

const form = ref({
  partNo: '',
  description: '',
  htsCode: ''
});

// Strictly enforce HTS format: NNNN.NN.NNNN
watch(() => form.value.htsCode, (newVal) => {
  if (!newVal) return;
  
  // 1. Remove non-digits
  let digits = newVal.replace(/\D/g, '');
  
  // 2. Limit to 10 digits
  if (digits.length > 10) {
    digits = digits.slice(0, 10);
  }
  
  // 3. Format into NNNN.NN.NNNN
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
  
  // 4. Update form (only if changed to avoid loop)
  if (newVal !== formatted) {
    form.value.htsCode = formatted;
  }
});

const errors = ref({
  partNo: '',
  description: '',
  htsCode: ''
});

// BR-08: HTS Code Format Validation (NNNN.NN.NNNN)
const htsRegex = /^\d{4}\.\d{2}\.\d{4}$/;
const isHtsValid = computed(() => !form.value.htsCode || htsRegex.test(form.value.htsCode));

const validateForm = () => {
  let valid = true;
  errors.value = { partNo: '', description: '', htsCode: '' };

  if (!form.value.partNo) {
    errors.value.partNo = 'part_create.validation.required';
    valid = false;
  }
  if (!form.value.description) {
    errors.value.description = 'part_create.validation.required';
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
    await partService.createPart(form.value);
    alert(router.app.config.globalProperties.$t('part_create.success'));
    router.push('/parts');
  } catch (error) {
    console.error('Failed to create part:', error);
  }
};
</script>

<template>
  <div class="page-wrapper">
    <div class="page-container">
      <header class="page-header">
        <button class="back-link" @click="router.back()">← {{ $t('common.back') }}</button>
        <h1>{{ $t('part_create.title') }}</h1>
      </header>

      <div class="form-layout">
        <Card class="form-card">
          <form @submit.prevent="handleSubmit">
            <!-- Part No -->
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
              <span v-if="errors.partNo" class="error-text" data-test="part-no-error">{{ $t(errors.partNo) }}</span>
            </div>

            <!-- HTS Code (BR-08) -->
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
              <span v-if="errors.htsCode" class="error-text" data-test="hts-code-error">{{ $t(errors.htsCode) }}</span>
              <small class="hint-text">{{ $t('part_create.hts_code_placeholder') }}</small>
            </div>

            <!-- Description (BR-21) -->
            <div class="form-group">
              <label>{{ $t('part_create.description') }} <span class="required-asterisk">*</span></label>
              <textarea 
                v-model="form.description" 
                :placeholder="$t('part_create.description_placeholder')"
                class="form-textarea"
                :class="{ 'is-invalid': errors.description }"
                data-test="description-input"
              ></textarea>
              <span v-if="errors.description" class="error-text" data-test="description-error">{{ $t(errors.description) }}</span>
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
  padding: 2.5rem;
  max-width: 1200px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

.page-header {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  margin-bottom: 2.5rem;
}

.back-link {
  background: none;
  border: none;
  color: var(--primary-color);
  cursor: pointer;
  font-weight: 500;
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

.form-input, .form-textarea {
  width: 100%;
  padding: 0.8rem;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  font-family: inherit;
  outline: none;
  transition: border-color 0.2s;
}

.form-textarea {
  height: 150px;
  resize: vertical;
}

.form-input:focus, .form-textarea:focus {
  border-color: var(--primary-color);
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
