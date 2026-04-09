<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { authService, UserRole } from '../../services/auth/auth';
import AppButton from '../../components/common/Button.vue';

/**
 * Login View Component with i18n support (具備多國語系支援的登入頁面)
 */

const { t } = useI18n();
const router = useRouter();
const username = ref('');
const password = ref('');
const errorMessage = ref('');

/**
 * Handle login action (處理登入動作)
 */
const handleLogin = () => {
  errorMessage.value = '';
  
  const success = authService.login(username.value, password.value);
  
  if (success) {
    const role = authService.state.role;
    if (role === UserRole.EMPLOYEE) {
      router.push('/employee');
    } else if (role === UserRole.CUSTOMER) {
      router.push('/customer');
    } else {
      router.push('/');
    }
  } else {
    // Use i18n for error message (使用 i18n 錯誤訊息)
    errorMessage.value = t('login.error_invalid');
  }
};
</script>

<template>
  <div class="login-container">
    <div class="login-card">
      <h2>{{ $t('login.title') }}</h2>
      
      <div class="form-group">
        <label for="username">{{ $t('login.account_label') }}</label>
        <input 
          id="username" 
          v-model="username" 
          type="text" 
          :placeholder="$t('login.account_placeholder')" 
          @keyup.enter="handleLogin"
        />
      </div>
      
      <div class="form-group">
        <label for="password">{{ $t('login.password_label') }}</label>
        <input 
          id="password" 
          v-model="password" 
          type="password" 
          :placeholder="$t('login.password_placeholder')" 
          @keyup.enter="handleLogin"
        />
      </div>
      
      <div v-if="errorMessage" class="error-msg">
        {{ errorMessage }}
      </div>
      
      <div class="actions">
        <AppButton 
          :label="$t('common.login')" 
          class="login-btn" 
          @click="handleLogin" 
        />
      </div>
      
      <div class="hint">
        <p>{{ $t('login.hint_employee') }}</p>
        <p>{{ $t('login.hint_customer') }}</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 80vh;
}

.login-card {
  width: 400px;
  padding: 2rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.logo-area {
  text-align: center;
  margin-bottom: 2rem;
}

.logo-area h1 {
  color: var(--primary-color, #00a8e2);
  font-size: 2.5rem;
  margin: 0;
}

.logo-area .subtitle {
  color: #666;
  font-size: 0.9rem;
  margin-top: 0.5rem;
}

h2 {
  text-align: center;
  margin-bottom: 2rem;
  color: var(--sidebar-color, #465363);
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
  font-size: 14px;
}

input {
  width: 100%;
  height: 40px;
  padding: 0 12px;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 14px;
}

input:focus {
  border-color: var(--primary-color, #00a8e2);
  outline: none;
}

.error-msg {
  color: #ff0033;
  font-size: 12px;
  margin-bottom: 1rem;
}

.actions {
  display: flex;
  justify-content: center;
  margin-top: 1rem;
}

.login-btn {
  width: 100%;
  height: 44px;
  border-radius: 4px;
}

.hint {
  margin-top: 2rem;
  padding-top: 1rem;
  border-top: 1px dashed #ccc;
  font-size: 12px;
  color: #666;
}
</style>
