<script setup>
    import { useQuasar } from 'quasar';
    import { ref } from 'vue';
    import { useRouter } from 'vue-router';

    const $q = useQuasar();
    const router = useRouter();

    const login = ref(null);
    const password = ref(null);
    const confirm = ref(null);
    const re = new RegExp("^[0-9A-Za-z\_\*\!]{5,50}$");

    function goBack() {
        router.go(-1);
    }

    async function onSubmit() {
        try {
            const data = new URLSearchParams(
            {
                "login": login.value,
                "password": password.value
            });

            console.log(login)
            console.log(data.toString())

            let response = await fetch('api/Client/Registration?' + data.toString(),
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                let resText = await response.text();

                if (resText.trim() === '') {
                    resText = 'None';
                }

                throw new Error(`Code: ${response.status} - ${response.statusText}.\nText: ${resText}`);
            }

            $q.notify({
                color: 'green-4',
                textColor: 'white',
                icon: 'cloud_done',
                message: 'Registrated!!!'
            });

            router.push("/");
        }
        catch (err) {
            $q.notify({
                color: 'red',
                textColor: 'white',
                icon: 'warning',
                message: err.message
            });

            console.error(err);
        }
    };

    function onReset() {
        login.value = null;
        password.value = null;
        confirm.value = null;
    };
</script>

<template>
    <div id="mainContainer">
        <q-card id="mainCard" flat bordered>
            <q-card-section>
                <div id="hederContainer">
                    <q-btn label="Back"
                           type="reset"
                           color="primary"
                           flat
                           @click="goBack()"/>
                    <div class="text-h4">Registration</div>
                </div>
            </q-card-section>
            <q-separator />
            <q-card-section>
                <q-form id="mainForm"  
                        @submit="onSubmit"
                        @reset="onReset">
                    <q-input class="inputFild" 
                             filled
                             v-model="login"
                             label="You login"
                             :rules="[ value => re.test(value) || 'The login must be Latin letters or numbers from 5 to 50 characters.']" />
                    <q-input class="inputFild" 
                             type="password"
                             filled
                             label="You password"
                             v-model="password"
                             :rules="[ value => re.test(value) || 'The password must be Latin letters or numbers from 5 to 50 characters.']">
                    </q-input>
                    <q-input class="inputFild"
                             type="password"
                             filled
                             v-model="confirm"
                             label="Confirm password"
                             :rules="[ value => password == value || 'The confirm must be equal the password.']" />
                    <div>
                        <q-btn label="Submit" 
                               type="submit" 
                               color="primary" />
                        <q-btn label="Reset" 
                               type="reset" 
                               color="primary" 
                               flat />
                    </div>
                </q-form>
            </q-card-section>
        </q-card>
    </div>
</template>

<style>
    #mainContainer
    {
        width: 100vw;
        height: 100vh;
        display: table-cell;
        vertical-align: middle;
    }

    #hederContainer
    {
        display: flex;
        justify-content: space-between;
    }

    #mainCard {
        margin: auto;
        width: 500px;
    }

    .inputFild
    {
        margin-bottom: 10px;
    }
</style>