import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/home/HomeView.vue'
import LoginView from '../views/auth/LoginView.vue'
import EmployeeView from '../views/employee/EmployeeView.vue'
import CustomerView from '../views/customer/CustomerView.vue'
import PartListView from '../views/part/PartListView.vue'
import PartDetailView from '../views/part/PartDetailView.vue'
import { UserRole } from '../services/auth/auth'
import { useAuthStore } from '../stores/auth'

/**
 * Router Configuration (路由配置)
 * Update by Gemini AI on 2026-04-15
 */
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: { requiresAuth: true }
    },
    {
      path: '/employee',
      name: 'employee',
      component: EmployeeView,
      meta: { requiresAuth: true, role: UserRole.DIMERCO }
    },
    {
      path: '/customer',
      name: 'customer',
      component: CustomerView,
      meta: { requiresAuth: true, role: UserRole.CUSTOMER }
    },
    {
      path: '/parts',
      name: 'parts',
      component: PartListView,
      meta: { requiresAuth: true, role: undefined }
    },
    {
      path: '/parts/:id',
      name: 'part-detail',
      component: PartDetailView,
      meta: { requiresAuth: true, role: undefined }
    }
  ]
})

/**
 * Navigation Guard (導航守衛)
 * Bilingual comments following CCH constitution.
 */
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore();
  const isAuthenticated = authStore.isAuthenticated;
  const userRole = authStore.userRole;

  // 1. Check if route requires authentication (檢查路由是否需要驗證)
  if (to.meta.requiresAuth && !isAuthenticated) {
    next({ name: 'login' });
  } 
  // 2. Check Role-Based Access Control (檢查角色存取控制)
  else if (to.meta.role && to.meta.role !== userRole) {
    next({ name: 'home' });
  }
  // 3. Prevent logged-in users from accessing login page (防止已登入使用者存取登入頁)
  else if (to.name === 'login' && isAuthenticated) {
    if (userRole === UserRole.DIMERCO || userRole === UserRole.DCB) {
      next({ name: 'employee' });
    } else {
      next({ name: 'customer' });
    }
  }
  else {
    next();
  }
});

export default router
