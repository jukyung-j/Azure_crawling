import datetime
import logging
import azure.functions as func
import json
import time
from . import run

def main(mytimer: func.TimerRequest, tablePath: func.Out[str]) -> None:
    utc_timestamp = datetime.datetime.utcnow().replace(
        tzinfo=datetime.timezone.utc).isoformat()
    if mytimer.past_due:
        logging.info('The timer is past due!')

    logging.info('Python timer trigger function ran at %s', utc_timestamp)
    tags = run.naver();
    data = {
        "Nation": tags[0],
        "Price": tags[1],
        "Change": tags[2],
        "Blind": tags[3],
        "PartitionKey": "exchange_rate",
        "RowKey": int(time.time()-99999999999)
    }

    tablePath.set(json.dumps(data))


  