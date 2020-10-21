<template>
  <div id="app">
    <img alt="Vue logo" src="./assets/logo.png">    
  </div>
</template>

<script>
import gql from 'graphql-tag'


export default {
  name: 'App',
  components: {
    
  },
  mounted: function() {
    //console.log(this.$apollo.queries.players);
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
        document: gql`subscription test{onPaneltyAssigned {firstName lastName}}`,
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
