import Vue from 'vue'
import App from './App.vue'
import { createProvider, onLogin } from './vue-apollo'
import "regenerator-runtime"; // https://stackoverflow.com/questions/60619067/vue-js-and-es7-referenceerror-regeneratorruntime-not-defined
import Keycloak from 'keycloak-js';

Vue.config.productionTip = false

let initOptions = {
  url: 'http://localhost:8080/auth',
  realm: 'master',
  clientId: 'test'
}





var keycloak = new Keycloak(initOptions);
keycloak.init()
.then(function(authenticated) {
  if (!authenticated) {
    keycloak.login();
  } else {
    window.keycloak = keycloak;
    const apolloProvider = createProvider();
    console.log(apolloProvider);
    //onLogin(apolloProvider.defaultClient, window.keycloak.token);
    new Vue({
      apolloProvider: apolloProvider,
      render: h => h(App)
    }).$mount('#app')
  }
});






