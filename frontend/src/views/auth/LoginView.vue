<script lang="ts">
export default {
  name: 'LoginView'
};
</script>

<script setup lang="ts">
import { authService, UserRole } from '@src/services/auth/auth';
import AppButton from '@src/components/common/Button.vue';
import DimercoLogo from '@src/assets/images/DimLogo_Color_background_Transparent.png';

/**
 * Login View Component (具備視覺升級與多國語系的登入頁面)
 * Update by Gemini AI on 2026-04-18: Explicit name for cache control and path alias refactor. (顯式組件名稱以利緩存控管，並重構路徑別名。)
 */

const router = useRouter();
const username = ref('');
const password = ref('');

/**
 * Handle login action (處理登入動作)
 */
const handleLogin = async () => {
  const success = await authService.login(username.value, password.value);
  
  if (success) {
    const role = authService.state.role;
    if (role === UserRole.DIMERCO || role === UserRole.DCB) {
      router.push('/employee');
    } else if (role === UserRole.CUSTOMER) {
      router.push('/customer');
    } else {
      router.push('/');
    }
  }
};
</script>

<template>
  <div class="login-page">
    <!-- Background Decoration -->
    <div class="bg-decoration"></div>
    
    <div class="login-container">
      <div class="login-card">
        <!-- Logo Section -->
        <div class="logo-section">
          <img :src="DimercoLogo" alt="Dimerco Logo" class="brand-logo" />
          <h1 class="app-title">{{ $t('common.app_display_name') }}</h1>
        </div>

        <div class="form-section">
          <h2>{{ $t('login.title') }}</h2>
          
          <div class="form-group">
            <label for="username">{{ $t('login.account_label') }}</label>
            <input 
              id="username" 
              v-model="username" 
              type="text" 
              :placeholder="$t('login.account_placeholder')" 
              autocomplete="username"
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
              autocomplete="current-password"
              @keyup.enter="handleLogin"
            />
          </div>
          
          <div class="actions">
            <AppButton 
              :label="$t('common.login')" 
              class="login-btn" 
              @click="handleLogin" 
            />
          </div>
        </div>
        
        <div class="hint-section">
          <p class="hint-title">{{ $t('common.search') }} Quick Access (測試帳號):</p>
          <p>{{ $t('login.hint_customer') }}</p>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <footer class="login-footer">
      {{ $t('common.copyright') }}
    </footer>
  </div>
</template>

<style scoped lang="scss">
.login-page {
  position: relative;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #003366 0%, #00a8e2 100%);
  overflow: hidden;
}

.bg-decoration {
  position: absolute;
  top: -10%;
  right: -5%;
  width: 500px;
  height: 500px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 50%;
  z-index: 0;
}

.login-container {
  z-index: 1;
  width: auto; /* Changed to auto to allow stretching (改為 auto 以允許撐開) */
  max-width: 95vw; /* Increased max-width for flexibility (增加最大寬度以提供彈性) */
  padding: 20px;
}

.login-card {
  background: rgba(255, 255, 255, 0.98);
  padding: 3rem 2.5rem;
  border-radius: 12px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.2);
  backdrop-filter: blur(10px);
  width: fit-content; /* Stretch to fit title width (撐開以符合標題寬度) */
  min-width: 450px; /* Set a reasonable minimum width (設定合理的最小寬度) */
  margin: 0 auto;
}

.logo-section {
  text-align: center;
  margin-bottom: 2.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;

  .brand-logo {
    width: 180px;
    height: auto;
    margin-bottom: 1rem;
  }

  .app-title {
    font-size: 1.4rem; /* Restored fixed size (恢復固定字級) */
    color: #333;
    font-weight: 600;
    margin: 0.5rem 0 0.2rem;
    white-space: nowrap; /* Prevent folding (防止摺行) */
    width: max-content; /* Ensure width fits the text (確保寬度符合文字) */
  }
}

h2 {
  font-size: 1.2rem;
  color: #444;
  margin-bottom: 1.5rem;
  text-align: center;
  position: relative;
  
  &::after {
    content: '';
    display: block;
    width: 40px;
    height: 3px;
    background: #00a8e2;
    margin: 8px auto 0;
    border-radius: 2px;
  }
}

.form-group {
  margin-bottom: 1.25rem;

  label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: 600;
    font-size: 0.85rem;
    color: #555;
  }

  input {
    width: 100%;
    height: 44px;
    padding: 0 12px;
    border: 1px solid #ddd;
    border-radius: 6px;
    font-size: 0.95rem;
    transition: all 0.3s ease;

    &:focus {
      border-color: #00a8e2;
      box-shadow: 0 0 0 3px rgba(0, 168, 226, 0.1);
      outline: none;
    }
  }
}

.login-btn {
  width: 100%;
  height: 48px;
  margin-top: 1rem;
  font-size: 1rem;
  font-weight: 600;
  background-color: #00a8e2;
  border: none;
  border-radius: 6px;
  color: white;
  cursor: pointer;
  transition: transform 0.2s ease;

  &:hover {
    filter: brightness(1.1);
    transform: translateY(-1px);
  }

  &:active {
    transform: translateY(0);
  }
}

.hint-section {
  margin-top: 2.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid #eee;
  font-size: 0.85rem; /* Increased font size slightly for better readability (略微增加字號以提高可讀性) */
  color: #666; /* Darkened color for better contrast (加深顏色以提高對比度) */
  line-height: 1.6;
  text-align: center; /* Center the hint message (將提示訊息置中) */

  .hint-title {
    font-weight: 600;
    color: #444;
    margin-bottom: 0.5rem;
  }
}

.login-footer {
  margin-top: 2rem;
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.7);
  z-index: 1;
}
</style>
