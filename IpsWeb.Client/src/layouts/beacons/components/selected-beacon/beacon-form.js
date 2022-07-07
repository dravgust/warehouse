import {useEffect, useState} from "react";
import { useFormik } from "formik";
import {useMutation, useQuery} from "react-query";
import * as yup from "yup";
// Soft UI Dashboard React components
import Stack from "@mui/material/Stack";
import {
  Icon,
  TextField,
  Box,
} from "@mui/material";
import * as auth from "auth-provider";
import { client } from "utils/api-client";
import Autocomplete from "@mui/material/Autocomplete";
import SuiAlert from "components/SuiAlert";
import SuiButton from "components/SuiButton";
import DeletePromt from "./delete-promt";
import CircularProgress from '@mui/material/CircularProgress';

const validationSchema = yup.object({
  macAddress: yup
    .string("Enter MAC address")
    .min(5, "MAC address should be of minimum 6 characters length"),
});

export default function BeaconForm({ onSave = () => {}, onDelete = () => {}, item = {}, products = [] }) {

  const saveItem = async (item) => {
    const token = await auth.getToken();
    const res = await client(`sites/beacon/set`, {
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

  const handleDelete = async (item) => {
    const token = await auth.getToken();
    try {
      await client(`sites/beacon/delete`, { data: item,  token });
      return onDelete();
    } catch (err) {
      console.log("delete-item", err);
    }
  };

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      macAddress: item ? item.macAddress : "",
      product: item.product ? item.product : {name: 'n/a', id: ""},
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
        const token = await auth.getToken();
        const res = await client(`items?page=1&size=1000`, {token});
        if(res.data) {
          setOptions([{name: 'n/a', id: ""}, ...res.data]);
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
        <SuiAlert style={{fontSize:"12px"}} color={"error"} dismissible>
          {mutation.error.title || mutation.error.error}
        </SuiAlert>
      )}

      <TextField
        fullWidth
        id="macAddress"
        name="macAddress"
        label="MacAddress"
        value={formik.values.macAddress}
        onChange={formik.handleChange}
        error={formik.touched.macAddress && Boolean(formik.errors.macAddress)}
        helperText={formik.touched.macAddress && formik.errors.macAddress}
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

      <Stack my={2} py={2} direction="row" spacing={1} justifyContent="end">
        <DeletePromt
          renderButton={(handleClickOpen) => (
            <SuiButton variant="text" color="error" onClick={handleClickOpen} disabled={!item.macAddress}>
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

