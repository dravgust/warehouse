import { useEffect, useState } from "react";
import { useFormik } from "formik";
import { useMutation } from "react-query";
import * as yup from "yup";
import Stack from "@mui/material/Stack";
import { Icon, TextField, Box } from "@mui/material";
import Autocomplete from "@mui/material/Autocomplete";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import DeletePromt from "./delete-promt";
import CircularProgress from "@mui/material/CircularProgress";
import { getProducts, setBeacon, deleteBeacon } from "services/warehouse-service";

const validationSchema = yup.object({
  macAddress: yup
    .string("Enter MAC address")
    .min(12, "MAC address should be of minimum 12 characters length"),
});

export default function BeaconForm({ onSave = () => {}, onDelete = () => {}, item = {} }) {
  const mutation = useMutation(setBeacon, {
    onSuccess: () => {
      formik.resetForm();
      return onSave();
    },
  });

  const handleDelete = async (item) => {
    try {
      await deleteBeacon(item);
      return onDelete();
    } catch (err) {
      console.log("delete-beacon", err);
    }
  };

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      macAddress: item ? item.macAddress : "",
      name: item.name ? item.name : "",
      product: item.product ? item.product : { name: "n/a", id: "" },
      metadata: item && item.metadata ? item.metadata : [],
    },
    validationSchema: validationSchema,
    onSubmit: (values) => {
      mutation.mutate(values);
    },
  });

  //-------------- autocomplete --------
  const [open, setOpen] = useState(false);
  const [options, setOptions] = useState([]);
  const loading = open && options.length === 0;

  useEffect(() => {
    let active = true;
    if (!loading) {
      return undefined;
    }
    (async () => {
      if (active) {
        const queryKey = ["", "1", "", "1000"];
        const res = await getProducts({ queryKey });
        if (res.items) {
          setOptions([{ name: "n/a", id: "" }, ...res.items]);
        }
      }
    })();
    return () => {
      active = false;
    };
  }, [loading]);

  useEffect(() => {
    if (!open) {
      setOptions([]);
    }
  }, [open]);
  //-------------- end autocomplete --------

  return (
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
          {mutation.error.title || mutation.error.error || "Some error occurred!"}
        </SuiAlert>
      )}

      <TextField
        fullWidth
        sx={{
          "& .MuiInputBase-input": { width: "100% !important" },
        }}
        id="macAddress"
        name="macAddress"
        label="MacAddress"
        value={formik.values.macAddress}
        onChange={formik.handleChange}
        error={formik.touched.macAddress && Boolean(formik.errors.macAddress)}
        helperText={formik.touched.macAddress && formik.errors.macAddress}
        InputProps={{
          readOnly: Boolean(item.macAddress),
        }}
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
        <Autocomplete
          id="asynchronous-demo"
          sx={{ width: 300 }}
          open={open}
          onOpen={() => {
            setOpen(true);
          }}
          onClose={() => {
            setOpen(false);
          }}
          onChange={(e, value) => {
            formik.setFieldValue("product", value);
          }}
          value={formik.values.product}
          isOptionEqualToValue={(option, value) => option.id === value.id}
          getOptionLabel={(option) => option.name}
          options={options}
          loading={loading}
          renderInput={(params) => (
            <TextField
              {...params}
              id="product"
              name="product"
              label="Product Name"
              error={formik.touched.product && Boolean(formik.errors.product)}
              helperText={formik.touched.product && formik.errors.product}
              InputProps={{
                ...params.InputProps,
                endAdornment: (
                  <>
                    {loading ? <CircularProgress color="inherit" size={20} /> : null}
                    {params.InputProps.endAdornment}
                  </>
                ),
              }}
            />
          )}
        />
        {item.product ? <Icon>link</Icon> : <Icon>link_off</Icon>}
      </Stack>

      {formik.values.metadata &&
        formik.values.metadata.map(({ key, value, type }, index) => (
          <TextField
            key={index}
            fullWidth
            label={key}
            id={`metadata[${index}].value`}
            name={`metadata[${index}].value`}
            value={value}
            type={type || "text"}
            InputLabelProps={{ shrink: true }}
            onChange={formik.handleChange}
            error={
              formik.touched.metadata &&
              formik.touched.metadata[index].value &&
              formik.errors.metadata &&
              formik.errors.metadata[index] &&
              Boolean(formik.errors.metadata[index].value)
            }
            helperText={
              formik.touched.metadata &&
              formik.touched.metadata[index].value &&
              formik.errors.metadata &&
              formik.errors.metadata[index] &&
              formik.errors.metadata[index].value
            }
          />
        ))}

      <Stack my={2} py={2} direction="row" spacing={1} justifyContent="end">
        <DeletePromt
          renderButton={(handleClickOpen) => (
            <SuiButton
              variant="text"
              color="error"
              onClick={handleClickOpen}
              disabled={!item.macAddress}
            >
              <Icon>delete</Icon>&nbsp;delete
            </SuiButton>
          )}
          onDelete={() => handleDelete(item)}
        />
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
  );
}
