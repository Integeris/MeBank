<script setup>
    const props = defineProps({
        name: String,
        avatarSrc: String,
        rating: Number
    });

    const srcURL = new URL(props.avatarSrc, import.meta.url).pathname;
</script>

<template>
    <q-card id="Review"
            class="my-card">
        <q-card-section>
            <div class="text-h5">
                <q-avatar>
                    <img :src="srcURL">
                </q-avatar>
                {{name}}
                <q-rating color="primary"
                          :modelValue="rating"
                          readonly />
            </div>
            <q-separator />
            <div id="reviewTextBox">
                <slot />
            </div>
        </q-card-section>
    </q-card>
</template>

<style>
    #Review
    {
        max-width: 500px;
    }
    #reviewTextBox
    {
        text-align: justify;
    }
</style>