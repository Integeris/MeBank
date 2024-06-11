<script setup>
    import HomeButton from '@/components/MenuHomeButton.vue';
</script>

<template>
    <header>
        <q-card id="menuCard" square flat>
            <q-card-section id="menuCardSelection" horizontal>
                <HomeButton />
                <slot/>
            </q-card-section>
        </q-card>
    </header>
</template>

<style>
    #menuCardSelection {
        padding: 0px;
        height: 55px;
        align-items: center;
    }
</style>