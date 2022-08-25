import { useSecurityController, setRoles } from "../context/security.context";
import { useClient } from "../context/auth.context";
//const localStorageKey = "__sec_provider_roles__";

const useSecurity = () => {
  const [, dispatch] = useSecurityController();
  const client = useClient();

  function handleSecurityResponse({ roles }) {
    //window.localStorage.setItem(localStorageKey, roles);
    console.log("sec-provider", roles);
    setRoles(dispatch, roles);
    return roles;
  }

  const fetchRoles = async () => {
    return await client(`security/user-roles`, {}).then(handleSecurityResponse);
  };

  return { fetchRoles };
};
export default useSecurity;
