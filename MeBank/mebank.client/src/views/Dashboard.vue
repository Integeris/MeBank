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

    const chartOptions = {
        chart: {
            type: "line",
            stacked: true
        },
        stroke: {
            curve: 'smooth',
        },
        dataLabels: {
            enable: false
        },
        xaxis: {
            type: 'datetime'
        }
    };

    // Список банковских счетов.
    let bankAccounts = ref([]);

    // Текущий банковский счёт.
    let currentBankAccount;

    // Показать диалоговое окно создания счёта.
    let createAccountShow = ref(false);

    // Паказать диалоговое окно перевода денег.
    let transferMoneyShow = ref(false);

    // Показать диалоговое окно конвертации валюты.
    let convertCurrencyShow = ref(false);

    // Список валют.
    let currencies = ref([]);

    // Выбранная валюта в диалоговом окне.
    let selectedCurrency;

    // Выбранный банковский счёт в диалоговом окне.
    let selectedBankAccount = ref(null);

    // Сумма для перевода денег.
    let amountTransfer = ref(null);

    // Транзакции текущего аккаунта.
    let entries = ref([]);

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
    let chartSeries = ref([]);

    await UpdateData();

    async function UpdateData() {
        bankAccounts.value = await GetBankAccounts();
        currentBankAccount = ref(bankAccounts.value[0]);
        entries.value = await GetBankAccountEntries(currentBankAccount.value);
        currencies.value = await GetCurrencies();
        selectedCurrency = ref(currencies.value[0]);

        chartSeries.value = [];

        for (var i = 0; i < bankAccounts.value.length; i++) {
            let entries = await GetBankAccountEntries(bankAccounts.value[i]);
            let data = [];

            for (var j = 0; j < entries.length; j++) {
                data.push(
                    {
                        x: new Date(entries[j].date),
                        y: entries[j].amount
                    }
                );
            }

            chartSeries.value.push(
                {
                    name: bankAccounts.value[i].idBankAccount,
                    data: data
                }
            );
        }
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

    async function ExecuteQuery(api, data, method, successText = null) {
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

            if (successText != null) {
                $q.notify({
                    color: 'green-4',
                    textColor: 'white',
                    icon: 'cloud_done',
                    message: successText
                });
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

    async function OnDialogTransferMoney() {
        transferMoneyShow.value = true;
    }

    async function OnTransferMoney() {
        const data = new URLSearchParams(
            {
                "login": login.value,
                "token": token.value,
                "idDebitBankAcount": currentBankAccount.value.idBankAccount,
                "idCreditBankAcount": selectedBankAccount.value,
                "amount": amountTransfer.value
            });

        await ExecuteQuery('api/Client/MoneyTransfer', data, "POST", "The funds have been transferred.");
        await UpdateData();

        selectedBankAccount.value = "";
    }

    async function OnDialogConvertCurrency() {
        convertCurrencyShow.value = true;
    }

    async function OnConvertCurrency() {

        const data = new URLSearchParams(
            {
                "login": login.value,
                "token": token.value,
                "idBankAccount": currentBankAccount.value.idBankAccount,
                "idCurrency": selectedCurrency.value.idCurrency
            });

        await ExecuteQuery('api/Client/BankAccountConversion', data, "POST", "Successful currency conversion.");
        await UpdateData();
        convertCurrencyShow.value = false;
    }

    async function OnDialogAddBankAccount() {
        createAccountShow.value = true;
    }

    async function OnAddBankAccount() {
        const data = new URLSearchParams(
        {
            "idCurrency": selectedCurrency.value.idCurrency,
            "login": login.value,
            "token": token.value
        });

        await ExecuteQuery('api/Client/AddBankAccount', data, "POST", "Account created!");
        await UpdateData();

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
                           @click="OnDialogTransferMoney"
                           color="primary"
                           label="Transfer money" />
                    <q-btn class="actionButton"
                           @click="OnDialogConvertCurrency"
                           color="primary"
                           label="Convert currency" />
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
        <div id="chartContainer">
            <h3>
                Cash flow
            </h3>
            <div id="filtersContainer">
                <!--Элементы фильтрации.-->
                <q-input v-model="date" filled type="date" hint="From date" />
                <q-input v-model="date" filled type="date" hint="To date" />
                <q-select v-model="model" :options="currencies" label="Currency" />
                <q-select v-model="model" :options="bankAccounts" label="Bank accounts" />
            </div>
            <apexchart height="300px"
                       :options="chartOptions"
                       :series="chartSeries" />
        </div>
        <q-dialog v-model="createAccountShow">
            <q-card>
                <q-card-section>
                    <h2>
                        Select bank account currency
                    </h2>
                </q-card-section>

                <q-card-section class="q-pt-none">
                    <q-select filled
                              v-model="selectedCurrency"
                              :options="currencies"
                              option-label="title"
                              label="Currency" />
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
        <q-dialog v-model="convertCurrencyShow">
            <q-card>
                <q-card-section>
                    <h2>
                        Select bank account currency
                    </h2>
                </q-card-section>

                <q-card-section class="q-pt-none">
                    <q-select filled
                              v-model="selectedCurrency"
                              :options="currencies"
                              option-label="title"
                              label="Currency" />
                </q-card-section>

                <q-card-actions align="right">
                    <q-btn flat
                           label="Back"
                           color="primary"
                           v-close-popup />
                    <q-btn @click="OnConvertCurrency"
                           flat
                           label="Convert"
                           color="primary"
                           v-close-popup />
                </q-card-actions>
            </q-card>
        </q-dialog>
        <q-dialog v-model="transferMoneyShow">
            <q-card>
                <q-card-section>
                    <h2>
                        Transfer money
                    </h2>
                </q-card-section>

                <q-card-section class="q-pt-none">
                    Bank account:
                    <q-input v-model="selectedBankAccount"
                             filled
                             label="Bank account"
                             :rules="[val => !Number.isNaN(Number(val)) || 'Only a numeric number can be entered.', val => Number(val) != currentBankAccount.idBankAccount || 'You can not transfer money to the same account.']" />
                    Amount:
                    <q-input v-model="amountTransfer"
                             filled
                             label="Bank account"
                             :rules="[val => !Number.isNaN(Number(val)) || 'Only a numeric number can be entered.', val => Number(val) > 0 || 'The transfer amount must be greater than zero.']" />
                </q-card-section>

                <q-card-actions align="right">
                    <q-btn flat
                           label="Back"
                           color="primary"
                           v-close-popup />
                    <q-btn @click="OnTransferMoney"
                           flat
                           label="Transfer"
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

    #filtersContainer
    {
        display: flex;
        gap: 20px;
    }

    #chartContainer {
        margin: 10px;
    }

    .actionButton
    {
        width: 100%;
    }
</style>