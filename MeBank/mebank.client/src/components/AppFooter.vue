<template>
    <footer>
        <div class="informationContainer">
            <div>
                <div class="header">
                    Contact us:
                </div>
                Email: support@mebank.com
                <br>
                Tel: 8-800-555-55-55
            </div>
            <div>
                <div class="header">
                    Legal information:
                </div>
                Privacy Policy
                <br>
                Personal data processing agreement
            </div>
            <div>
                <div class="header">
                    Social Networks:
                </div>
                Twitter
                <br>
                X
            </div>
        </div>
    </footer>
</template>

<style>
    footer {
        padding: 20px 30px;
        background-color: #C7C7C7;
        color: #333333;
    }

    .header
    {
        font-size: 20px;
    }

    .informationContainer {
        justify-content: center;
        display: flex;
        grid-gap: 20%;
    }
</style>