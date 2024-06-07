<script setup>
    import { ref } from 'vue'
    import homeButton from '@/components/MenuHomeButton.vue';
    import CardView from '@/components/CardView.vue'

    const services = ref([
        {
            title: "Deposit",
            description: "Rates up to 16% per annum in rubles with the possibility of replenishment or partial withdrawal.Commission fee not more than 0.3%"
        },
        {
            title: "Internet Bank",
            description: "Allows customers to manage their accounts and make transactions online directly on the website"
        },
        {
            title: "Credit",
            description: "Rates up to 16% per annum in rubles with the possibility of early repayment. Commission fee not more than 0.5%"
        }
    ])
</script>

<template>
    <header>
        <q-card id="menuCard" class="my-card" square flat>
            <q-card-section id="menuCardSelection" horizontal>
                <homeButton />
                <q-tabs id="navigationMenu"
                        class="text-primary"
                        v-model="tab">
                    <q-route-tab href="#AboutUs" name="AboutUs" label="About Us" />
                    <q-route-tab href="#Services" name="Services" label="Services" />
                    <q-route-tab href="#Reviews" name="Reviews" label="Reviews" />
                </q-tabs>
                <div class="signContainer">
                    <q-btn class="menuButton" to="/AppAuthorization" no-caps flat color="primary" label="Sign in" />
                    <q-btn class="menuButton" to="/AppRegistration" no-caps color="primary" label="Sign up" />
                </div>
            </q-card-section>
        </q-card>
    </header>
    <main>
        <q-img id="startImage"
               src="../assets/images/BankPrevew.jpg">
            <div class="absolute-bottom-right text-subtitle2">
                Bank Branch "MeBank"
            </div>
        </q-img>
        <q-card id="AboutUs"
                class="my-card"
                square flat>
            <q-card-section id="AboutUsBankSelection">
                <div id="aboutUsBody">
                    <div id="textBlock">
                        <div class="text-h4">
                            About Us
                        </div>
                        <q-separator />
                        <p>
                            We are a startup bank created to revolutionize the banking system. Our mission is to provide innovative and convenient banking solutions
                            for individuals and businesses. We focus on security, comfort and personalized service to help customers take control of their finances.
                            We have online banking and a mobile app that allows customers to access a wide range of banking services online. Our goal is to create
                            a convenient and safe banking experience for our customers.
                        </p>
                        <p>
                            Our team consists of experienced banking and technology professionals who work to create innovative solutions for our customers.
                            We are constantly evolving and improving our services to provide the best experience for our customers.
                        </p>
                        <p>
                            We believe that banking services should be accessible and understandable for everyone. Therefore, we create simple and transparent
                            products that help our customers achieve their financial goals. Our bank is your reliable partner in the world of finance.
                        </p>
                        <br />
                        <div>
                            <span id="aboutUsSignature">
                                <b>
                                    Director Harrison J. D.
                                </b>
                            </span>
                        </div>
                    </div>
                    <q-card class="my-card">
                        <q-avatar id="aboutUsUsEmployeeAvatar">
                            <img src="../assets/images/Employee.jpg" />
                        </q-avatar>
                    </q-card>
                </div>
            </q-card-section>
        </q-card>
        <q-card id="Services"
                class="my-card"
                square flat>
            <q-card-section id="servicesSelection">
                <div class="text-h4">
                    Services
                </div>
                <div id="servicesBody">
                    <CardView v-for="service in services"
                              :width="300"
                              :title="service.title">
                        {{service.description}}
                    </CardView>
                </div>
            </q-card-section>
        </q-card>
        <q-card id="Reviews"
                class="my-card"
                square flat>
        </q-card>
    </main>
</template>

<style scoped>
    #menuCardSelection {
        padding: 0px;
        height: 55px;
        align-items: center;
    }

    #startImage
    {
        height: 500px;  
    }

    #AboutUs {
        margin: 15px 150px 15px 50px;
    }

    #aboutUsBankSelection {
        padding: 10px;
    }

    #aboutUsBody
    {
        display: inline-flex;
        align-items: center;
        font-size: 16px;
    }

    #aboutUsBody #textBlock {
        margin-right: 60px;
    }

    #textBlock {
        text-align:justify;
    }

    #aboutUsSignature
    {
        font-size: 14px;
        display: flex;
        justify-content: right;
    }

    #aboutUsUsEmployeeAvatar {
        width: 250px;
        height: 250px;
    }

    #Services {
        text-align: center;
    }

    #servicesSelection {
        flex-direction: column;
        display: flex;
        justify-content: space-around;
    }

    #servicesBody {
        width: 100%;
        display: flex;
    }

    #serviceItem
    {
        width: 300px;
    }

    .signContainer {
        margin: auto 5px auto auto;
    }

    .menuButton {
        height: 45px;
        font-size: 15px;
    }
</style>
