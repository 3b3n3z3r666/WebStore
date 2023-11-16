import { Grid, Typography } from '@mui/material';
import { useAppSelector } from '../../app/store/configureStore';
import BasketSummary from '../basket/BasketSummary';
import BasketTable from '../basket/BasketTable';



export default function Review() {
  const { basket } = useAppSelector(state => state.basket)
  return (
    <>
      <Typography variant="h6" gutterBottom>
        Order summary
      </Typography>
      {basket &&
        <BasketTable items={basket.items} />
      }
      <Grid container>
        <Grid item xs={12} />
        <Grid item xs={12}>
          <BasketSummary />

        </Grid>

      </Grid>
    </>
  );
}