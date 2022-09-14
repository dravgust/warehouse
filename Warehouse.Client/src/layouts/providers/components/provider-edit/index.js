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
import { saveProvider } from "api/admin";

const ProviderEdit = ({ item, onSave, onClose }) => {
  const mutation = useMutation(saveProvider, {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });
  const validationSchema = yup.object({
    name: yup.string("Enter name").required("Name is required"),
    alias: yup.string("Enter alias").required("Alias is required"),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      id: item ? item.id : 1000,
      name: item ? item.name : "",
      alias: item ? item.alias : "",
      description: item ? item.description : "",
      culture: item ? item.culture : "",
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate({ ...values });
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
              id="name"
              name="name"
              label="Name"
              value={formik.values.name}
              onChange={formik.handleChange}
              error={formik.touched.name && Boolean(formik.errors.name)}
              helperText={formik.touched.name && formik.errors.name}
            />

            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              id="description"
              name="description"
              label="Description"
              value={formik.values.description}
              onChange={formik.handleChange}
              error={formik.touched.description && Boolean(formik.errors.description)}
              helperText={formik.touched.description && formik.errors.description}
            />

            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              autoComplete="off"
              autoFocus="true"
              type="text"
              id="alias"
              name="alias"
              label="Alias"
              value={formik.values.alias}
              onChange={formik.handleChange}
              error={formik.touched.alias && Boolean(formik.errors.alias)}
              helperText={formik.touched.alias && formik.errors.alias}
            />

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
ProviderEdit.defaultProps = {};

// Typechecking props
ProviderEdit.propTypes = {
  item: PropTypes.object.isRequired,
  onClose: PropTypes.func.isRequired,
};

export default ProviderEdit;
