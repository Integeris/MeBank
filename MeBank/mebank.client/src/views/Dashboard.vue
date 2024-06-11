<script setup>
    import AppFooter from "@/components/AppFooter.vue";
    import HeaderMenu from "@/components/HeaderMenu.vue";
</script>

<template>
    <HeaderMenu>

    </HeaderMenu>
    <main id="dashboardMainContainer">

    </main>
    <AppFooter/>
</template>

<style scoped>
    #dashboardMainContainer
    {
        display: flex;
        padding: 30px 10px 10px 10px;
    }

    #dashboardLeftPanel
    {
        display: flex;
        flex-direction: column;
    }
</style>