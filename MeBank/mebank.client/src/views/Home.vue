<script setup>
    import { ref } from 'vue'
    import homeButton from '@/components/MenuHomeButton.vue';
    import CardView from '@/components/CardView.vue'
    import ReviewBox from "@/components/ReviewBox.vue"
    import AppFooter from "@/components/AppFooter.vue"

    const navigationSectionItems = ref([
        {
            name: "AboutUs",
            label: "About Us"
        },
        {
            name: "Services",
            label: "Services"
        },
        {
            name: "Reviews",
            label: "Reviews"
        }
    ]);

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
    ]);

    const reviews = ref([
        {
            name: "Gabriel Green",
            avatarSrc: "../assets/images/ReviewAvatar1.jpg",
            rating: "5",
            text: `My dealings with the bank have been extremely positive. The bank staff was courteous, attentive and professional.
            They answered my questions in detail and assisted in processing transactions. I was also pleased that the bank
            offers convenient card terms including grace period and freebies. Overall, I recommend this bank to anyone looking
            for reliable and comfortable banking service.`
        },
        {
            name: "Emily Thompson",
            avatarSrc: "../assets/images/ReviewAvatar2.jpg",
            rating: "4",
            text: `In my experience with banking institutions, this bank stands out for its efficient organization and high 
            level of service. The bank's staff demonstrates excellent financial skills and knowledge, enabling them to effectively assist 
            customers in achieving their financial goals. I was also impressed with the wide range of financial products and services the bank 
            offers, including individual accounts, investment instruments, and loan programs. Overall, I recommend this bank to anyone looking for a 
            reliable and professional banking partner.`
        },
        {
            name: "Michael Johnson",
           avatarSrc: "../assets/images/ReviewAvatar3.jpg",
            rating: "5",
            text: `My company needed additional resources to expand production and the bank offered us excellent terms for a loan. 
            The bank staff was courteous and professional, helping me with the loan application and answering my questions. 
            I was pleased that the bank offered the best terms and flexible repayment options. Overall, I recommend this bank 
            to anyone looking for a reliable and comfortable banking partner to finance their business initiatives.`
        },
    ]);
</script>

<template>
    <header>
        <q-card id="menuCard" class="my-card" square flat>
            <q-card-section id="menuCardSelection" horizontal>
                <homeButton />
                <q-tabs id="navigationMenu"
                        class="text-primary">
                    <q-route-tab v-for="item in navigationSectionItems"
                                 :href="'#' + item.name"
                                 :label="item.label" />
                </q-tabs>
                <div class="signContainer">
                    <q-btn class="menuButton"
                           to="/AppAuthorization"
                           color="primary"
                           label="Sign in"
                           no-caps flat />
                    <q-btn class="menuButton"
                           to="/AppRegistration"
                           color="primary"
                           label="Sign up"
                           no-caps />
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
                        <div class="text-h3">
                            About Us
                            <q-separator />
                        </div>
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
                    </div>
                    <div class="directorContainer">
                        <q-avatar id="aboutUsUsEmployeeAvatar">
                            <img src="../assets/images/Employee.jpg" />
                        </q-avatar>
                        <div>
                            <b>
                                Director Harrison J. D.
                            </b>
                        </div>
                    </div>
                </div>
            </q-card-section>
        </q-card>
        <q-card id="Services"
                class="my-card"
                square flat>
            <q-card-section id="servicesSelection">
                <div class="text-h3">
                    Services
                </div>
                <div id="servicesBody">
                    <CardView class="servicesCard"
                              v-for="service in services"
                              :title="service.title">
                        {{service.description}}
                    </CardView>
                </div>
            </q-card-section>
        </q-card>
        <q-card id="Reviews"
                class="my-card"
                square flat>
            <q-card-section>
                <div class="text-h3">
                    Reviews
                </div>
                <div id="reviewsBody">
                    <ReviewBox class="reviewsCard"
                               v-for="review in reviews"
                               :name="review.name"
                               :avatarSrc="review.avatarSrc"
                               :rating="review.rating">
                        {{review.text}}
                    </ReviewBox>
                </div>
            </q-card-section>
        </q-card>
    </main>
    <AppFooter/>
</template>

<style scoped>
    p {
        margin-bottom: 10px;
    }

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
        margin: 15px 100px 15px 0px;
    }

    #aboutUsBody
    {
        display: flex;
        font-size: 16px;
    }

    #aboutUsBody #textBlock {
        margin-right: 60px;
        padding: 0px;
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

    #servicesSelection {
        flex-direction: column;
        display: flex;
    }

    #servicesBody {
        width: 100%;
        display: flex;
        justify-content: space-evenly;
    }

    #serviceItem
    {
        width: 300px;
    }

    #reviewsBody {
        display: flex;
        justify-content: space-evenly;
    }

    .signContainer {
        margin-left: auto;
    }

    .menuButton {
        height: 45px;
        font-size: 15px;
    }

    .text-h3
    {
        margin-bottom: 20px;
    }

    .directorContainer {
        margin: auto;
        text-align: center;
    }

    .servicesCard {
        width: 500px;
    }
</style>
