import {
  Card,
  Icon,
  Tooltip,
  IconButton,
  Box,
  TextField,
  Stack,
  Autocomplete,
} from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { useFormik } from "formik";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import { useMutation } from "react-query";
import * as yup from "yup";
import * as auth from "auth-provider";
import { client } from "utils/api-client";

// prop-types is a library for typechecking of props
import PropTypes from "prop-types";

export default function SetGateway({
  item,
  onClose,
  onSave = () => {},
  gwRegistered = [],
}) {
  const saveItem = async (item) => {
    const token = await auth.getToken();
    const res = await client(`sites/set-gateway`, {
      data: item,
      token,
    });
    return res;
  };

  const mutation = useMutation((item) => saveItem(item), {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const validationSchema = yup.object({
    name: yup
      .string("Enter product name")
      .min(3, "Name should be of minimum 3 characters length")
      .required("Name is required"),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      macAddress: item ? item.macAddress : "",
      name: item ? item.name : "",
      circumscribedRadius: item ? item.circumscribedRadius : 0,
      location: item ? item.location : 0,
      envFactor: item ? item.envFactor : 0,
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
          <SuiTypography variant="h6">Set Gateway</SuiTypography>
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

            <Autocomplete
              disablePortal
              options={["", ...gwRegistered]}
              isOptionEqualToValue={(option, value) => option === value}
              sx={{ width: 300 }}
              getOptionLabel={(option) => option}
              onChange={(e, value) => {
                formik.setFieldValue("macAddress", value);
              }}
              value={formik.values.macAddress}
              renderInput={(params) => (
                <TextField
                  id="macAddress"
                  name="macAddress"
                  label="MacAddress"
                  {...params}
                  error={formik.touched.macAddress && Boolean(formik.errors.macAddress)}
                  helperText={formik.touched.macAddress && formik.errors.macAddress}
                />
              )}
            />
            <Stack direction="row" spacing={2} alignItems="center">
              <TextField
                fullWidth
                id="name"
                name="name"
                label="Name"
                value={formik.values.name}
                onChange={formik.handleChange}
                error={formik.touched.name && Boolean(formik.errors.name)}
                helperText={formik.touched.name && formik.errors.name}
              />
            </Stack>
            <Stack direction="row" spacing={2} alignItems="center">
              <TextField
                fullWidth
                id="envFactor"
                name="envFactor"
                label="envFactor"
                value={formik.values.envFactor}
                onChange={formik.handleChange}
                error={formik.touched.envFactor && Boolean(formik.errors.envFactor)}
                helperText={formik.touched.envFactor && formik.errors.envFactor}
              />
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
}

// Setting default values
SetGateway.defaultProps = {};

// Typechecking props
SetGateway.propTypes = {
  item: PropTypes.object.isRequired,
  onClose: PropTypes.func.isRequired,
};
