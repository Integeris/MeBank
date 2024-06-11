<script setup>
    import {reactive} from 'vue';
    import Dashboard from '@/views/Dashboard.vue';
    import Welcome from '@/views/Welcome.vue';

    const state = reactive({
        token: sessionStorage.token || null    
    });

    // Нужно создать запрос на проверку токена, если он есть.
</script>

<template>
    <Dashboard/>
    <!--<Dashboard v-if="state.token != null"/>
    <Welcome v-if="state.token == null"/>-->
</template>