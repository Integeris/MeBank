<script setup>
    const props = defineProps({
        title: "String"
    });
</script>   

<template>
    <q-card id="card" 
            class="my-card">
        <q-card-section id="cardSelection">
            <div class="text-h5">
                <div id="titleBox" class="text-h5">
                    {{title}}
                </div>
            </div>
            <q-separator />
            <div id="textBlock">
                <slot/>
            </div>
        </q-card-section>
    </q-card>
</template>

<style>
    #cardSelection
    {
        padding: 0px;
    }

    #titleBox
    {
        text-align: center;
    }

    #textBlock {
        padding: 20px;
        margin: auto;
    }
</style>