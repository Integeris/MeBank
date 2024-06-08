import '@quasar/extras/roboto-font/roboto-font.css'

// Import icon libraries
import '@quasar/extras/material-icons/material-icons.css'
import '@quasar/extras/material-icons-sharp/material-icons-sharp.css'
import 'quasar/src/css/index.sass'

import { createApp } from 'vue';
import { Quasar } from 'quasar'
import quasarLang from 'quasar/lang/ru'
import App from './App.vue';
import router from "./router/index.js";

const app = createApp(App);
app.use(router);
app.use(Quasar, {
    plugins: {}, // import Quasar plugins and add here
    lang: quasarLang
})

app.mount('#app');