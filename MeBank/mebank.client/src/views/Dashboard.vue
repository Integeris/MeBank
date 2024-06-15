<script setup>
    import { useStorage } from "@vueuse/core"
    import { useQuasar } from 'quasar';
    import AppFooter from "@/components/AppFooter.vue";
    import HeaderMenu from "@/components/HeaderMenu.vue";
    import VueApexCharts from 'vue-apexcharts'
    import { useRouter } from 'vue-router';
    import { ref, reactive } from 'vue'

    const $q = useQuasar();
    const login = useStorage('login', undefined, sessionStorage);
    const token = useStorage('token', undefined, sessionStorage);

    // Список банковских счетов.
    let bankAccounts = ref([]);

    // Текущий банковский счёт.
    let currentBankAccount;

    // Показать диалоговое окно создания счёта.
    let createAccountShow = ref(false);

    // Список валют.
    let currencies = ref([]);

    // Выбранная валюта в диалоговом окне.
    let currentCurrency;

    // Транзакции текущего аккаунта.
    let entries = ref([]);

    await UpdateData();

    // Колонки транзакций.
    const entryColumns = [
        {
            name: 'IdBankAccount',
            required: true,
            label: 'Bank account',
            align: 'center',
            field: row => row.idBankAccount,
            format: val => `${val}`,
            sortable: true
        },
        {
            name: 'Amount',
            required: true,
            label: 'Amount',
            align: 'center',
            field: row => row.amount,
            format: val => `${val}`,
            sortable: true
        },
        {
            name: 'Date',
            required: true,
            label: 'Date',
            align: 'center',
            field: row => {
                const date = new Date(row.date);

                let day = date.getDate();
                let month = (date.getMonth() < 10) ? '0' + date.getMonth() : date.getMonth();
                let year = date.getFullYear();
                let hours = date.getHours();
                let minutes = (date.getMinutes() < 10) ? '0' + date.getMinutes() : date.getMinutes();
                let seconds = date.getSeconds()

                return day + '.' + month + '.' + year + ' ' + hours + ':' + minutes + ':' + seconds;
            },
            format: val => `${val}`,
            sortable: true
        }
    ];

    // Диаграмма.
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

    async function UpdateData() {
        bankAccounts.value = await GetBankAccounts();
        currentBankAccount = ref(bankAccounts.value[0]);
        entries.value = await GetBankAccountEntries(currentBankAccount.value);
        currencies.value = await GetCurrencies();
        currentCurrency = ref(currencies.value[0]);
    }

    async function GetBankAccounts()
    {
        const data = new URLSearchParams(
        {
            "login": login.value,
            "token": token.value
        });

        return await ExecuteQuery('api/Client/GetBankAccounts', data, "GET");
    }

    async function GetBankAccountEntries(bankAccount)
    {
        const data = new URLSearchParams(
        {
            "login": login.value,
            "token": token.value,
            "idBankAccount": bankAccount.idBankAccount
        });

        return await ExecuteQuery('api/Client/GetBankAccountEntries', data, "GET");
    }

    async function GetCurrencies() {
        const data = new URLSearchParams({ });
        return await ExecuteQuery('api/Client/GetCurrencies', data, "GET");
    }

    async function ExecuteQuery(api, data, method) {
        try {
            let response = await fetch(api + "?" + data.toString(),
            {
                method: method,
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            let result = await response.json();

            if (!response.ok) {
                throw new Error(`Code: ${response.status} - ${response.statusText}.\nText: ${result}`);
            }

            return result;
        } catch (err) {
            console.error(err);

            $q.notify({
                color: 'red',
                textColor: 'white',
                icon: 'warning',
                message: err.message
            });
        }
    }

    async function OnChangeSelected(bankAccount) {
        entries.value = await GetBankAccountEntries(bankAccount);
    }

    async function OnTransferMoney() {

    }

    async function OnDialogAddBankAccount() {
        createAccountShow.value = true;
    }

    async function OnAddBankAccount() {
        const data = new URLSearchParams(
        {
            "idCurrency": currentCurrency.value.idCurrency,
            "login": login.value,
            "token": token.value
        });

        await ExecuteQuery('api/Client/AddBankAccount', data, "POST");
        await UpdateData();

        $q.notify({
            color: 'green-4',
            textColor: 'white',
            icon: 'cloud_done',
            message: 'Account created!'
        });

        createAccountShow.value = false;
    }

    function OnSignOutClick() {
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
            <q-btn @click="OnSignOutClick()" 
                   color="primary" 
                   label="Sign out" />
        </div>
    </HeaderMenu>
    <main>
        <div id="accountsContainer">
            <div id="accountData">
                <q-select filled
                          @update:model-value="OnChangeSelected"
                          v-model="currentBankAccount"
                          :options="bankAccounts"
                          option-label="idBankAccount"
                          label="Bank account" />
                <div id="balanceTextBox">
                    Balance: {{currentBankAccount.balance}} {{currentBankAccount.currencyTitle}}.
                </div>
                <div id="actionsContainer">
                    <q-btn class="actionButton"
                           @click="OnTransferMoney"
                           color="primary"
                           label="Transfer money" />
                    <q-btn class="actionButton"
                           @click="OnDialogAddBankAccount"
                           color="primary"
                           label="Add bank account" />
                </div>
            </div>
            <div id="entriesContainer">
                <q-table title="Entries"
                         :rows="entries"
                         :columns="entryColumns"
                         row-key="idEntry" />
            </div>
        </div>
        <div>
            <!--<apexchart :options="chartOptions" :series="chartSeries"></apexchart>-->
        </div>
        <q-dialog v-model="createAccountShow">
            <q-card>
                <q-card-section>
                    <h2>
                        Change bank account currency
                    </h2>
                </q-card-section>

                <q-card-section class="q-pt-none">
                    <q-select filled
                              v-model="currentCurrency"
                              :options="currencies"
                              option-label="title"
                              label="Bank account" />
                </q-card-section>

                <q-card-actions align="right">
                    <q-btn flat 
                           label="Back" 
                           color="primary" 
                           v-close-popup />
                    <q-btn @click="OnAddBankAccount"
                           flat 
                           label="Create" 
                           color="primary" 
                           v-close-popup />
                </q-card-actions>
            </q-card>
        </q-dialog>
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
        font-size: 40px;
    }

    #actionsContainer
    {
        height: 70px;
        display: flex;
        gap: 10px;
    }

    #entriesContainer {
        margin: 10px 10px 10px 10px;
        display: inline-block;
        width: 50%;
    }

    .actionButton
    {
        width: 100%;
    }
</style>