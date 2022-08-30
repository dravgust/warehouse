import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { Box, Card, Icon, IconButton, Stack, TextField, Tooltip } from "@mui/material";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import { useMutation } from "react-query";
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

const roles = ["Support", "Administrator", "Supervisor"];

function getStyles(name, userRoles, theme) {
  return {
    fontWeight:
      userRoles.indexOf(name) === -1
        ? theme.typography.fontWeightRegular
        : theme.typography.fontWeightMedium,
    margin: "5px",
  };
}

const UserEdit = ({ item, onSave, onClose }) => {
  const theme = useTheme();
  const [userRoles, setUserRoles] = React.useState([]);

  const handleChange = (event) => {
    const {
      target: { value },
    } = event;
    setUserRoles(typeof value === "string" ? value.split(",") : value);
  };

  const mutation = useMutation(() => {}, {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const validationSchema = yup.object({
    name: yup
      .string("enter username")
      .min(3, "username should be of minimum 3 characters length")
      .required("username is required"),
    email: yup.string("enter email").email(),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: item ? item.id : "",
      username: item ? item.username : "",
      password: item ? item.password : "",
      email: item ? item.email : "",
      phone: item ? item.phone : "",
      type: item ? item.type : "",
      providerId: item ? item.providerId : "",
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate(values);
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
              id="username"
              name="username"
              label="Username"
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
              id="password"
              name="password"
              label="Password"
              type="password"
              value={formik.values.password}
              onChange={formik.handleChange}
              error={formik.touched.password && Boolean(formik.errors.password)}
              helperText={formik.touched.password && formik.errors.password}
            />

            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              id="email"
              name="email"
              label="Email"
              value={formik.values.email}
              onChange={formik.handleChange}
              error={formik.touched.email && Boolean(formik.errors.email)}
              helperText={formik.touched.email && formik.errors.email}
            />

            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              id="phone"
              name="phone"
              label="Phone"
              type="number"
              value={formik.values.phone}
              onChange={formik.handleChange}
              error={formik.touched.phone && Boolean(formik.errors.phone)}
              helperText={formik.touched.phone && formik.errors.phone}
            />

            <Stack direction="row" spacing={2} alignItems="center" p={1}>
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
                  <MenuItem value={"Support"}>Support</MenuItem>
                  <MenuItem value={"Administrator"}>Administrator</MenuItem>
                  <MenuItem value={"Supervisor"}>Supervisor</MenuItem>
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
                  <MenuItem value={2}>Dolav</MenuItem>
                </Select>
              </FormControl>
            </Stack>

            <FormControl
              fullWidth
              sx={{
                m: 1,
                "& .MuiInputBase-input": { width: "100% !important", minHeight: "40px" },
              }}
            >
              <InputLabel id="label_roles">User Roles</InputLabel>
              <Select
                labelId="label_roles"
                id="select_roles"
                multiple
                value={userRoles}
                onChange={handleChange}
                input={<OutlinedInput id="input_roles" label="User Roles" />}
                renderValue={(selected) => (
                  <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5, paddingTop: "5px" }}>
                    {selected.map((value) => (
                      <Chip key={value} label={value} />
                    ))}
                  </Box>
                )}
                MenuProps={MenuProps}
              >
                {roles.map((name) => (
                  <MenuItem key={name} value={name} style={getStyles(name, userRoles, theme)}>
                    {name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

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
