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

const UserEdit = ({ item, onSave, onClose }) => {
  console.log("user", item);
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
      email: item ? item.email : "",
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
              id="email"
              name="email"
              label="Email"
              value={formik.values.email}
              onChange={formik.handleChange}
              error={formik.touched.email && Boolean(formik.errors.email)}
              helperText={formik.touched.email && formik.errors.email}
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
