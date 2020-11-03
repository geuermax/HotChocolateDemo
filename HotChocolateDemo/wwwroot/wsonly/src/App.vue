<template>
  <div id="app">
    <img alt="Vue logo" src="./assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js App"/>
  </div>
</template>

<script>
import HelloWorld from './components/HelloWorld.vue'
import gql from 'graphql-tag'

export default {
  name: 'App',
  components: {
    HelloWorld
  },
  apollo: {
    players: {
      query: gql`query players{
        players {
          id
          firstName
          lastName
          penalties {
            id
            name
          }
        }
      }`,
      update: response => {
        console.log(response);
      },
      subscribeToMore: {
        document: gql`subscription test{onPenaltyAssigned {firstName lastName}}`,
        updateQuery: (prevResult, {subscriptionData}) => {
          console.log('Subscription: ', subscriptionData)
        }
      }
    }
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
