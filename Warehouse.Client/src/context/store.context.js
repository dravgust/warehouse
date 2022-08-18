import { createContext, useContext, useReducer, useMemo } from "react";
import PropTypes from "prop-types";

const Store = createContext(null);
Store.displayName = "StoreContext";

const actionMap = {
  RESOURCES: (state, action) => ({ ...state, resources: action.value }),
  SITE: (state, action) => ({ ...state, site: action.value }),
  PRODUCT: (state, action) => ({ ...state, product: action.value }),
};

function reducer(state, action) {
  const handler = actionMap[action.type];
  return handler ? handler(state, action) : state;
}

function StoreControllerProvider({ children }) {
  const initialState = {
    resources: [],
    site: null,
    product: null,
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

const setResources = (dispatch, value) => dispatch({ type: "RESOURCES", value });
const setSite = (dispatch, value) => dispatch({ type: "SITE", value });
const setProduct = (dispatch, value) => dispatch({ type: "PRODUCT", value });

export { StoreControllerProvider, useStoreController, setResources, setSite, setProduct };
