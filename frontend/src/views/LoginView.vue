<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { authService, UserRole } from '../services/auth';
import AppButton from '../components/common/Button.vue';

/**
 * Login View Component (登入頁面組件)
 */

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
    // Redirect based on role (根據角色重導向)
    if (role === UserRole.EMPLOYEE) {
      router.push('/employee');
    } else if (role === UserRole.CUSTOMER) {
      router.push('/customer');
    } else {
      router.push('/');
    }
  } else {
    // Bilingual error message (雙語錯誤訊息)
    errorMessage.value = 'Invalid account or password. (帳號或密碼錯誤)';
  }
};
</script>

<template>
  <div class="login-container">
    <div class="login-card">
      <h2>Login (登入)</h2>
      
      <div class="form-group">
        <label for="username">Account (帳號)</label>
        <input 
          id="username" 
          v-model="username" 
          type="text" 
          placeholder="Enter Account (請輸入帳號)" 
          @keyup.enter="handleLogin"
        />
      </div>
      
      <div class="form-group">
        <label for="password">Password (密碼)</label>
        <input 
          id="password" 
          v-model="password" 
          type="password" 
          placeholder="Enter Password (請輸入密碼)" 
          @keyup.enter="handleLogin"
        />
      </div>
      
      <div v-if="errorMessage" class="error-msg">
        {{ errorMessage }}
      </div>
      
      <div class="actions">
        <!-- Using existing Button component (使用現有的 Button 組件) -->
        <AppButton 
          label="Login (登入)" 
          class="login-btn" 
          @click="handleLogin" 
        />
      </div>
      
      <div class="hint">
        <p>Employee (員工): Y9999 / 888888</p>
        <p>Customer (客戶): customer001 / 888888</p>
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
