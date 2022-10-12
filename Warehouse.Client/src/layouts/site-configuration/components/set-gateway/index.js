import {
  Card,
  Icon,
  Tooltip,
  IconButton,
  Box,
  TextField,
  Stack,
  Autocomplete,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import { useFormik } from "formik";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import { useMutation } from "react-query";
import * as yup from "yup";
import PropTypes from "prop-types";
import { setSiteGw } from "api/warehouse";

const locations = [
  "Unknown",
  "Center",
  "TopCenter",
  "TopLeft",
  "TopRight",
  "BottomCenter",
  "BottomLeft",
  "BottomRight",
  "CenterLeft",
  "CenterRight",
];

export default function SetGateway({
  item,
  onClose,
  onSave = () => {},
  gateways = [],
  beacons = [],
}) {
  const saveItem = async (values) => {
    const item = {
      siteId: values.siteId,
      macAddress: values.macAddress,
      name: values.name,
      circumscribedRadius: values.circumscribedRadius,
      location: values.location,
      envFactor: values.envFactor,
      gauge: {
        mac: values.macG,
        txPower: values.txPowerG,
        radius: values.radiusG,
      },
    };

    return await setSiteGw(item);
  };

  const mutation = useMutation((values) => saveItem(values), {
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
      envFactor: item ? item.envFactor : 1,
      macG: item && item.gauge ? item.gauge.mac : "",
      radiusG: item && item.gauge ? item.gauge.radius : 0,
      txPowerG: item && item.gauge ? item.gauge.txPower : 0,
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate({ ...values, siteId: item.siteId });
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
              options={["", ...gateways]}
              isOptionEqualToValue={(option, value) => option === value}
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

            <TextField
              fullWidth
              sx={{
                "& .MuiInputBase-input": { width: "100% !important" },
              }}
              id="name"
              name="name"
              label="Name"
              value={formik.values.name}
              onChange={formik.handleChange}
              error={formik.touched.name && Boolean(formik.errors.name)}
              helperText={formik.touched.name && formik.errors.name}
            />
            <Stack direction="row" spacing={2} alignItems="center">
              <TextField
                fullWidth
                sx={{
                  "& .MuiInputBase-input": { width: "100% !important" },
                }}
                id="envFactor"
                name="envFactor"
                label="EnvFactor"
                type={"number"}
                value={formik.values.envFactor}
                onChange={formik.handleChange}
                error={formik.touched.envFactor && Boolean(formik.errors.envFactor)}
                helperText={formik.touched.envFactor && formik.errors.envFactor}
              />

              <FormControl
                fullWidth
                sx={{
                  "& .MuiInputBase-input": { width: "100% !important" },
                }}
              >
                <InputLabel id="location-label">Location</InputLabel>
                <Select
                  labelId="location-label"
                  id="location"
                  name="location"
                  value={formik.values.location}
                  label="Location"
                  onChange={(e) => {
                    formik.setFieldValue("location", e.target.value);
                  }}
                >
                  {locations.map((l, i) => (
                    <MenuItem key={`loc-${i}`} value={i} sx={{ minWidth: 120 }}>
                      {l}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Stack>
            <SuiBox>
              <SuiTypography fontSize="small">Gauge</SuiTypography>
            </SuiBox>
            <Autocomplete
              options={["", ...beacons]}
              isOptionEqualToValue={(option, value) => option === value}
              getOptionLabel={(option) => option}
              onChange={(e, value) => {
                formik.setFieldValue("macG", value);
              }}
              value={formik.values.macG}
              renderInput={(params) => (
                <TextField
                  id="macG"
                  name="macG"
                  label="MacAddress"
                  {...params}
                  error={formik.touched.macG && Boolean(formik.errors.macG)}
                  helperText={formik.touched.macG && formik.errors.macG}
                />
              )}
            />
            <Stack direction="row" spacing={2} alignItems="center">
              <TextField
                fullWidth
                sx={{
                  "& .MuiInputBase-input": { width: "100% !important" },
                }}
                type="number"
                id="radiusG"
                name="radiusG"
                label="Radius"
                value={formik.values.radiusG}
                onChange={formik.handleChange}
                error={formik.touched.radiusG && Boolean(formik.errors.radiusG)}
                helperText={formik.touched.radiusG && formik.errors.radiusG}
              />
              <TextField
                fullWidth
                sx={{
                  "& .MuiInputBase-input": { width: "100% !important" },
                }}
                type={"number"}
                id="txPowerG"
                name="txPowerG"
                label="TxPower"
                value={formik.values.txPowerG}
                onChange={formik.handleChange}
                error={formik.touched.txPowerG && Boolean(formik.errors.txPowerG)}
                helperText={formik.touched.txPowerG && formik.errors.txPowerG}
              />
            </Stack>

            <Stack my={2} px={1} direction="row" spacing={1} justifyContent="end">
              <SuiButton color="secondary" variant="contained" onClick={onClose}>
                cancel
              </SuiButton>
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
