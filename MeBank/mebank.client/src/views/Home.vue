<script setup>

</script>

<template>
    <header>
        <q-card class="my-card">
            <q-card-section id="menuCard">
                <q-btn to="/" id="menuHomeButton" flat label="MeBank"/>
            </q-card-section>
        </q-card>
    </header>
</template>

<style scoped>
    #menuCard
    {
        padding: 0px;
        height: 55px;
    }

    #menuHomeButton {
        height: 55px;
        color: #3184de;
    }
</style>
