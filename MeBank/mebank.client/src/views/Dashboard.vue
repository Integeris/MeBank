<script setup>
    import { useStorage } from "@vueuse/core"
    import AppFooter from "@/components/AppFooter.vue";
    import HeaderMenu from "@/components/HeaderMenu.vue";
    import { useRouter } from 'vue-router';
    import { ref } from 'vue'

    const login = useStorage('login', undefined, sessionStorage);
    const token = useStorage('token', undefined, sessionStorage);

    let bankAccounts = [
        "1231",
        "1231",
        "1231dfs"
    ];

    const currentBankAccount = ref(bankAccounts[0]);

    function onClick()
    {
        login.value = null;
        token.value = null;
    }
</script>

<template>
    <HeaderMenu>
        <div id="accountInformationContainer">
            <div id="userLoginContainer">
                <div id="userLogin">
                    Login: {{login}}
                </div>
            </div>
            <q-btn @click="onClick" 
                   color="primary" 
                   label="Sign out" />
        </div>
    </HeaderMenu>
    <main>
        <div id="accountsContainer">
            <div id="accountData">
                <q-select filled
                          v-model="currentBankAccount"
                          :options="bankAccounts"
                          label="Bank account" />
                <div>
                    Balance
                </div>
                <div>
                    Currency
                </div>
            </div>
            <div id="paymentStory">
                Table of Transfers
            </div>
        </div>
    </main>
    <AppFooter/>
</template>

<style scoped>
    #accountInformationContainer
    {
        margin: auto 10px auto auto;
        display: flex;
        gap: 20px
    }

    #userLoginContainer
    {
        display: table;
    }

    #userLogin
    {
        display: table-cell;
        vertical-align: middle;
    }

    #accountsContainer
    {

    }

    #accountData {
        display: inline-block;
        width: 50%;
    }

    #paymentStory {
        display: inline-block;
        width: 50%;
    }
</style>