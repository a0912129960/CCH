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
      meta: { requiresAuth: true, title: 'common.home' }
    },
    {
      path: '/employee',
      name: 'employee',
      component: EmployeeView,
      // Both DIMERCO and DCB share the same Dashboard view
      meta: { requiresAuth: true, roles: [UserRole.DIMERCO, UserRole.DCB], title: 'employee.title' }
    },
    {
      path: '/customer',
      name: 'customer',
      component: CustomerView,
      meta: { requiresAuth: true, role: UserRole.CUSTOMER, title: 'customer.title' }
    },
    {
      path: '/parts',
      name: 'parts',
      component: PartListView,
      meta: { requiresAuth: true, title: 'part_list.title' }
    },
    {
      path: '/parts/new',
      name: 'part-create',
      component: () => import('../views/part/PartCreateView.vue'),
      meta: { requiresAuth: true, title: 'part_create.title' }
    },
    {
      path: '/parts/upload',
      name: 'part-upload',
      component: () => import('../views/part/BulkUploadView.vue'),
      meta: { requiresAuth: true, title: 'part_upload.title' }
    },
    {
      path: '/parts/:id',
      name: 'part-detail',
      component: PartDetailView,
      meta: { requiresAuth: true, title: 'part_detail.title' }
    }
  ]
})

router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore();
  const isAuthenticated = authStore.isAuthenticated;
  const userRole = authStore.userRole;

  if (to.meta.requiresAuth && !isAuthenticated) {
    next({ name: 'login' });
  }
  // Single-role restriction (e.g. /customer → CUSTOMER only)
  else if (to.meta.role && to.meta.role !== userRole) {
    next({ name: 'home' });
  }
  // Multi-role restriction (e.g. /employee → DIMERCO or DCB)
  else if (to.meta.roles && !(to.meta.roles as string[]).includes(userRole as string)) {
    next({ name: 'home' });
  }
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
