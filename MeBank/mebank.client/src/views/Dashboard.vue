<script setup>
    import { useStorage } from "@vueuse/core"
    import AppFooter from "@/components/AppFooter.vue";
    import HeaderMenu from "@/components/HeaderMenu.vue";
    import VueApexCharts from 'vue-apexcharts'
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

    const entryColumns = [
        {
            name: 'IdBankAccount',
            required: true,
            label: 'Bank account',
            align: 'center',
            field: row => row.IdBankAccount,
            format: val => `${val}`,
            sortable: true
        },
        {
            name: 'Balance',
            required: true,
            label: 'Balance',
            align: 'center',
            field: row => row.Balance,
            format: val => `${val}`,
            sortable: true
        },
        {
            name: 'CurrencyTitle',
            required: true,
            label: 'Currency',
            align: 'center',
            field: row => row.CurrencyTitle,
            format: val => `${val}`,
            sortable: true
        }
    ];

    const entries = [
        {
            IdBankAccount: 1,
            Balance: 100,
            CurrencyTitle: "RUB"
        }
    ];

    const chartSeries = [
        {
            name: "Payment",
            data: [{ x: '05/06/2014', y: 54 }, { x: '05/08/2014', y: 17 }, { x: '05/28/2014', y: 26 }]
        },
        {
            name: "GetPayment",
            data: [{ x: '05/06/2014', y: 20 }, { x: '05/08/2014', y: 40 }, { x: '05/28/2014', y: 26 }]
        }
    ]

    const chartOptions = {
        chart: {
            type: "bar",
            stacked: true
        },
        plotOptions: {
            bar: {
                horizontal: false,
                endingShape: "rounded"
            }
        },
        dataLabels: {
            enable: false
        },
        xaxis: {
            type: 'datetime'
        }
    }

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
                <div id="balanceTextBox">
                    Balance: 31231
                </div>
                <div>
                    Currency: RUB
                </div>
            </div>
            <div id="entriesContainer">
                <q-table title="Entries"
                         :rows="entries"
                         :columns="entryColumns"
                         row-key="IdBankAccount" />
            </div>
        </div>
        <div>
            <apexchart :options="chartOptions" :series="chartSeries"></apexchart>
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

    #accountsContainer {
        display: flex;
    }

    #accountData {
        margin: 10px 10px 10px 10px;
        display: inline-block;
        width: 50%;
    }

    #balanceTextBox
    {
        font-size: 45px;
    }

    #entriesContainer {
        margin: 10px 10px 10px 10px;
        display: inline-block;
        width: 50%;
    }
</style>