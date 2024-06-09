import { createRouter, createWebHistory } from 'vue-router';
import Home from '../views/Home.vue';
import Authorization from '../views/AppAuthorization.vue';
import Registration from '../views/AppRegistration.vue';

const routes = [
    {
        path: "/",
        name: "Home",
        component: Home
    },
    {
        path: "/Authorization",
        name: "Authorization",
        component: Authorization
    },
    {
        path: "/Registration",
        name: "Registration",
        component: Registration
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes: routes
});

export default router;