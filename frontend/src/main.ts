import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import './scss/style.scss'
import App from './App.vue'
import router from './router'
import i18n from './locales' // Import i18n (匯入 i18n)

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(i18n) // Use i18n (使用 i18n)
app.use(ElementPlus)

app.mount('#app')
