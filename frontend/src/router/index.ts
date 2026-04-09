import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import EmployeeView from '../views/EmployeeView.vue'
import CustomerView from '../views/CustomerView.vue'
import PartListView from '../views/PartListView.vue'
import PartDetailView from '../views/PartDetailView.vue'
import { authService, UserRole } from '../services/auth'

/**
 * Router Configuration (路由配置)
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
      meta: { requiresAuth: true, role: UserRole.EMPLOYEE }
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
      meta: { requiresAuth: true, role: UserRole.CUSTOMER }
    },
    {
      path: '/parts/:id',
      name: 'part-detail',
      component: PartDetailView,
      meta: { requiresAuth: true, role: UserRole.CUSTOMER }
    }
  ]
})

/**
 * Navigation Guard (導航守衛)
 * Bilingual comments following CCH constitution.
 */
router.beforeEach((to, from, next) => {
  const isAuthenticated = authService.isAuthenticated();
  const userRole = authService.state.role;

  // 1. Check if route requires authentication (檢查路由是否需要驗證)
  if (to.meta.requiresAuth && !isAuthenticated) {
    // Redirect to login if not authenticated (未驗證時重新導向至登入頁)
    next({ name: 'login' });
  } 
  // 2. Check Role-Based Access Control (檢查角色存取控制)
  else if (to.meta.role && to.meta.role !== userRole) {
    // Redirect to home if role mismatch (角色不符時重新導向至首頁)
    next({ name: 'home' });
  }
  // 3. Prevent logged-in users from accessing login page (防止已登入使用者存取登入頁)
  else if (to.name === 'login' && isAuthenticated) {
    if (userRole === UserRole.EMPLOYEE) {
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
