<script setup>
    import { useStorage } from '@vueuse/core'
    import {reactive} from 'vue';
    import Dashboard from '@/views/Dashboard.vue';
    import Welcome from '@/views/Welcome.vue';
    import { ref } from 'vue';

    let login;
    let token;

    updatePage();

    async function validationAsync() {
        try {
            const data = new URLSearchParams(
                {
                    "login": login.value,
                    "token": token.value
                });

            let response = await fetch('api/Client/ValidateToken?' + data.toString(),
            {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                let result = await response.json();
                throw new Error(`Code: ${response.status} - ${response.statusText}.\nText: ${result}`);
            }

            return true;

        } catch (err) {
            sessionStorage.removeItem("login");
            sessionStorage.removeItem("token");

            console.error(err);
        }

        return false;
    }

    function updatePage() {
        login = useStorage('login', undefined, sessionStorage);
        token = useStorage('token', undefined, sessionStorage);

        if (token.value != undefined) {
            validationAsync();
        }
    }
</script>

<template>
    <Suspense>
        <Dashboard v-if="login != undefined" />
    </Suspense>
    <Welcome v-if="login == undefined"/>
</template>