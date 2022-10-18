import json, logging, os
import resources as aws

logging.basicConfig(format='%(asctime)s - %(message)s', level=logging.INFO)


def main():
    key_id = os.environ.get('AWS_SECRET_KEY_ID', 'xxx')
    session = aws.create_session(
        key_id,
        os.environ.get('AWS_SECRET_ACCESS_KEY', 'xxx'),
        os.environ.get('REGION_NAME', 'us-east-1')
    )

    suffix = os.environ.get('SUFFIX', '-local')

    ## Create SQS queues
    sqs = aws.create_client(session, 'sqs', key_id, '4576')
    sqs_queue = aws.create_sqs_queue(sqs, f'online-store{suffix}')
