FROM mongo:3.6

WORKDIR /data
ADD defaultdata defaultdata
ADD cmds cmds

RUN chmod +x cmds/run.sh
RUN chmod +x cmds/set_mongodb_users_and_Data.sh

CMD ["/data/cmds/run.sh"]