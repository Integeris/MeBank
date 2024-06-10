<script setup>
    import {reactive} from 'vue';
    import AuthorizedHome from '@/views/AuthorizedHome.vue';
    import Welcome from '@/views/Welcome.vue';

    const state = reactive({
        token: sessionStorage.token || null    
    });;
</script>

<template>
    <AuthorizedHome v-if="state.token != null"/>
    <Welcome v-if="state.token == null"/>
</template>