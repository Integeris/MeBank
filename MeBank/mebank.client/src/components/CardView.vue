<script setup>
    import { watch } from 'vue'

    const props = defineProps({
        title: "Null",
        width: "300"
    })

    watch(props.width, (value) =>
        document
            .documentElement
            .style
            .setProperty('--width', value + "px"),
    {
        immediate: true
    })
</script>   

<template>
    <q-card id="card" 
            class="my-card">
        <q-card-section>
            <div class="text-h5">
                <div class="text-h5">
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
    #card {
        width: 100%;
        max-width: var(--width);
    }

    #textBlock
    {
        font-size: 14px;
    }
</style>