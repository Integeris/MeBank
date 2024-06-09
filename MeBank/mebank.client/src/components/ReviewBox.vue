<script setup>
    const props = defineProps({
        name: "String",
        avatarSrc: "String",
        rating: "String"
    });

    const srcURL = new URL(props.avatarSrc, import.meta.url).pathname;
</script>

<template>
    <q-card id="Reviews"
            class="my-card">
        <q-card-section>
            <div class="text-h5">
                <q-avatar>
                    <img :src="srcURL">
                </q-avatar>
                {{name}}
                <q-rating color="primary"
                          size="20px"
                          :max="'5'"
                          :model-value="rating"
                          readonly />
            </div>
            <q-separator />
            <div id="textBox">
                <slot />
            </div>
        </q-card-section>
    </q-card>
</template>

<style>
    #textBox
    {
        text-align: justify;
    }
</style>