import { createContext, useContext, useReducer, useMemo } from "react";
import PropTypes from "prop-types";

const Store = createContext(null);

Store.displayName = "StoreContext";

function reducer(state, action) {
  switch (action.type) {
    case "RESOURCES":
      return { ...state, resources: action.value };
  }
}

function StoreControllerProvider({ children }) {
  const initialState = {
    resources: [],
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

export { StoreControllerProvider, useStoreController, setResources };
