import { createContext, useContext, useReducer, useMemo } from "react";
import PropTypes from "prop-types";

const Store = createContext(null);
Store.displayName = "StoreContext";

const actionMap = {
  SITE: (state, action) => ({ ...state, site: action.value }),
  PRODUCT: (state, action) => ({ ...state, product: action.value }),
  BEACON: (state, action) => ({ ...state, beacon: action.value }),
};

function reducer(state, action) {
  const handler = actionMap[action.type];
  return handler ? handler(state, action) : state;
}

function StoreControllerProvider({ children }) {
  const initialState = {
    site: null,
    product: null,
    beacon: null,
  };
  const [controller, dispatch] = useReducer(reducer, initialState);
  const value = useMemo(() => [controller, dispatch], [controller, dispatch]);
  return <Store.Provider value={value}>{children}</Store.Provider>;
}

function useStoreController() {
  const context = useContext(Store);
  if (!context) {
    throw new Error("useStoreController should be used inside the StoreControllerProvider.");
  }
  return context;
}

StoreControllerProvider.propTypes = {
  children: PropTypes.node.isRequired,
};

const setSite = (dispatch, value) => dispatch({ type: "SITE", value });
const setProduct = (dispatch, value) => dispatch({ type: "PRODUCT", value });
const setBeacon = (dispatch, value) => dispatch({ type: "BEACON", value });

export { StoreControllerProvider, useStoreController, setSite, setProduct, setBeacon };
