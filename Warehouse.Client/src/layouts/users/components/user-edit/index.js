import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { Box, Card, Grid, Icon, IconButton, Stack, TextField, Tooltip } from "@mui/material";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import { useMutation, useQuery } from "react-query";
import * as yup from "yup";
import { useFormik } from "formik";
import PropTypes from "prop-types";
import React from "react";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import FormControl from "@mui/material/FormControl";
import Select from "@mui/material/Select";
import { useTheme } from "@mui/material/styles";
import OutlinedInput from "@mui/material/OutlinedInput";
import Chip from "@mui/material/Chip";
import { getRoles, getUserRoles, saveUser } from "api/admin";
import { fetchRoles, fetchUserRoles } from "utils/query-keys";
import useSecurity, { SecurityPermissions } from "services/security-provider";
import { queryClient } from "context/app.context";

const ITEM_HEIGHT = 48;
const ITEM_PADDING_TOP = 8;
const MenuProps = {
  PaperProps: {
    style: {
      maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
      width: 250,
    },
  },
};

function getStyles(id, userRoles, theme) {
  return {
    fontWeight: Boolean(userRoles.find((item) => item.id === id))
      ? theme.typography.fontWeightRegular
      : theme.typography.fontWeightMedium,
    margin: "5px",
  };
}

const UserEdit = ({ item, onSave, onClose }) => {
  const theme = useTheme();
  const { hasPermissions } = useSecurity("USER", SecurityPermissions.Grant);
  const [selectedRoles, setSelectedRoles] = React.useState([]);

  const { data: roles, isSuccess: isRolesSuccess } = useQuery([fetchRoles], getRoles);
  const { data: userRoles, isSuccess: isUserRolesSuccess } = useQuery(
    [fetchUserRoles, item],
    getUserRoles,
    { enabled: Boolean(item.id) }
  );

  React.useEffect(() => {
    isRolesSuccess &&
      isUserRolesSuccess &&
      setSelectedRoles(
        roles.items.filter((item) => {
          return Boolean(userRoles.items.find((role) => item.id == role.id));
        })
      );
  }, [item, isRolesSuccess, isUserRolesSuccess]);

  const handleChange = (event) => {
    const {
      target: { value },
    } = event;
    setSelectedRoles(typeof value === "string" ? value.split(",") : value);
  };

  const mutation = useMutation(saveUser, {
    onSuccess: () => {
      formik.resetForm();
      queryClient.resetQueries(fetchUserRoles, { exact: false });
      return onSave();
    },
  });

  const validationSchema = yup.object({
    username: yup.string("Enter email").required("Username is required").email(),
    phone: yup
      .string("Enter phone number")
      .required("phone is required")
      .matches(/^05\d{8}$/, {
        message: "Please enter valid number.",
        excludeEmptyString: false,
      }),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: item ? item.id : 0,
      username: item ? item.username : "",
      password: "",
      phone: item ? item.phone : "",
      type: item ? item.type : "",
      providerId: item ? item.providerId : "",
      logLevel: item && item.logLevel ? item.logLevel : 0,
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate({ ...values, roles: selectedRoles.map((role) => role.id) });
    },
  });

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6">Edit User</SuiTypography>
        </SuiBox>
        <SuiBox display="flex" alignItems="center" mt={{ xs: -1, sm: 0 }}>
          <IconButton size="xl" color="inherit" onClick={onClose}>
            <Tooltip title="Close">
              <Icon>close</Icon>
            </Tooltip>
          </IconButton>
        </SuiBox>
      </SuiBox>
      <SuiBox
        component="div"
        display="flex"
        justifyContent="space-between"
        alignItems="flex-start"
        bgColor="rgba(203, 12, 159, 0.08)"
        borderRadius="lg"
        mb={2}
        mx={2}
        px={2}
        pt={2}
        style={{ border: "1px solid rgba(0, 0, 0, 0.125)" }}
      >
        <SuiBox width="100%" display="flex" flexDirection="column">
          <Box
            component="form"
            onSubmit={formik.handleSubmit}
            sx={{
              "& .MuiTextField-root": { m: 1 },
            }}
            noValidate
            autoComplete="off"
          >
            {mutation.isError && (
              <SuiAlert style={{ fontSize: "12px" }} color={"error"} dismissible>
                {mutation.error.title || mutation.error.error}
              </SuiAlert>
            )}
            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              autoComplete="off"
              autoFocus="true"
              type="text"
              id="username"
              name="username"
              label="Email"
              value={formik.values.username}
              onChange={formik.handleChange}
              error={formik.touched.username && Boolean(formik.errors.username)}
              helperText={formik.touched.username && formik.errors.username}
            />

            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              id="phone"
              name="phone"
              label="Phone"
              value={formik.values.phone}
              onChange={formik.handleChange}
              error={formik.touched.phone && Boolean(formik.errors.phone)}
              helperText={formik.touched.phone && formik.errors.phone}
            />

            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              autoComplete="off"
              type="password"
              id="password"
              name="password"
              label="Password"
              value={formik.values.password || ""}
              onChange={formik.handleChange}
              error={formik.touched.password && Boolean(formik.errors.password)}
              helperText={formik.touched.password && formik.errors.password}
            />

            <Grid container spacing={2}>
              <Grid item xs={12} xl={6}>
                <Stack direction="column" spacing={2} alignItems="center" p={1}>
                  <FormControl
                    fullWidth
                    sx={{
                      "& .MuiInputBase-input": { width: "100% !important" },
                    }}
                  >
                    <InputLabel id="type">User Type</InputLabel>
                    <Select
                      labelId="type"
                      id="type"
                      name="type"
                      value={formik.values.type}
                      label="type"
                      onChange={formik.handleChange}
                    >
                      <MenuItem value={"DeviceUser"}>DeviceUser</MenuItem>
                      <MenuItem value={"Support"}>Support</MenuItem>
                      <MenuItem value={"Developer"}>Developer</MenuItem>
                      <MenuItem value={"Administrator"}>Administrator</MenuItem>
                      <MenuItem value={"Supervisor"}>Supervisor</MenuItem>
                      <MenuItem value={"TechnicalUser"}>TechnicalUser</MenuItem>
                      <MenuItem value={"HealthChecker"}>HealthChecker</MenuItem>
                      <MenuItem value={"Guest"}>Guest</MenuItem>
                    </Select>
                  </FormControl>
                  <FormControl
                    fullWidth
                    sx={{
                      "& .MuiInputBase-input": { width: "100% !important" },
                    }}
                  >
                    <InputLabel id="providerId">Provider</InputLabel>
                    <Select
                      labelId="providerId"
                      id="providerId"
                      name="providerId"
                      value={formik.values.providerId}
                      label="providerId"
                      onChange={formik.handleChange}
                    >
                      <MenuItem value={1000}>Vayosoft</MenuItem>
                      <MenuItem value={4}>Tel-Aviv University</MenuItem>
                      <MenuItem value={3}>Meitav</MenuItem>
                      <MenuItem value={2}>Dolav</MenuItem>
                      <MenuItem value={1}>Electra</MenuItem>
                    </Select>
                  </FormControl>
                  <FormControl
                    fullWidth
                    sx={{
                      "& .MuiInputBase-input": { width: "100% !important" },
                    }}
                  >
                    <InputLabel id="logLevel">Log Level</InputLabel>
                    <Select
                      labelId="logLevel"
                      id="logLevel"
                      name="logLevel"
                      value={formik.values.logLevel}
                      label="logLevel"
                      onChange={formik.handleChange}
                    >
                      <MenuItem value={0}>Debug</MenuItem>
                      <MenuItem value={1}>Info</MenuItem>
                      <MenuItem value={2}>Warning</MenuItem>
                      <MenuItem value={3}>Error</MenuItem>
                      <MenuItem value={4}>CriticalError</MenuItem>
                    </Select>
                  </FormControl>
                </Stack>
              </Grid>
              <Grid item xs={12} xl={6}>
                <FormControl
                  fullWidth
                  sx={{
                    m: 1,
                    "& .MuiInputBase-input": { width: "100% !important", minHeight: "128px" },
                  }}
                >
                  <InputLabel id="label_roles">User Roles</InputLabel>
                  <Select
                    labelId="label_roles"
                    id="select_roles"
                    multiple
                    value={selectedRoles}
                    onChange={handleChange}
                    input={<OutlinedInput id="input_roles" label="User Roles" />}
                    renderValue={(selected) => (
                      <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5, paddingTop: "5px" }}>
                        {selected.map((value) => {
                          const val = typeof value === "string" ? value : value.name;
                          return <Chip key={val} label={val} />;
                        })}
                      </Box>
                    )}
                    MenuProps={MenuProps}
                  >
                    {isRolesSuccess &&
                      roles.items.map((role) => (
                        <MenuItem
                          key={role.id}
                          value={role}
                          style={getStyles(role.id, selectedRoles, theme)}
                        >
                          {role.name}
                        </MenuItem>
                      ))}
                  </Select>
                </FormControl>
              </Grid>
            </Grid>

            <Stack my={2} px={1} direction="row" spacing={1} justifyContent="end">
              <SuiButton color="success" variant="contained" type="submit">
                {mutation.isLoading ? (
                  "Loading..."
                ) : (
                  <>
                    <Icon>save</Icon>&nbsp;save
                  </>
                )}
              </SuiButton>
            </Stack>
          </Box>
        </SuiBox>
      </SuiBox>
    </Card>
  );
};

// Setting default values
UserEdit.defaultProps = {};

// Typechecking props
UserEdit.propTypes = {
  item: PropTypes.object.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default UserEdit;
