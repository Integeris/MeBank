<script setup>
    import { useStorage } from '@vueuse/core'
    import {reactive} from 'vue';
    import Dashboard from '@/views/Dashboard.vue';
    import Welcome from '@/views/Welcome.vue';
    import { ref } from 'vue';

    const login = useStorage('login', undefined, sessionStorage);
    const token = useStorage('token', undefined, sessionStorage);
</script>

<template>
    <Suspense>
        <Dashboard v-if="login != undefined" />
    </Suspense>
    <Welcome v-if="login == undefined"/>
</template>