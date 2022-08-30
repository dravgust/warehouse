import { createContext, useContext, useReducer, useMemo } from "react";
import PropTypes from "prop-types";

const Resources = createContext(null);
Resources.displayName = "Resources";

const actionMap = {
  RESOURCES: (state, action) => ({ ...state, resources: action.value }),
};

function reducer(state, action) {
  const handler = actionMap[action.type];
  return handler ? handler(state, action) : state;
}

function ResourcesProvider({ children }) {
  const initialState = {
    resources: [],
  };
  const [controller, dispatch] = useReducer(reducer, initialState);
  const value = useMemo(() => [controller, dispatch], [controller, dispatch]);
  return <Resources.Provider value={value}>{children}</Resources.Provider>;
}

function useResources() {
  const context = useContext(Resources);
  if (!context) {
    throw new Error("useResources should be used inside the ResourcesProvider.");
  }
  return context;
}

ResourcesProvider.propTypes = {
  children: PropTypes.node.isRequired,
};

const setResources = (dispatch, value) => dispatch({ type: "RESOURCES", value });

export { ResourcesProvider, useResources, setResources };
